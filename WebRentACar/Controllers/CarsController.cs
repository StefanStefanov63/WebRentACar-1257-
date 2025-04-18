﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebRentACar.Data;
using WebRentACar.Models;

namespace WebRentACar.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Cars
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.Include(c => c.CarBrand).Include(p => p.CarPictures).ToListAsync();
            ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name");
            return View( cars);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int CarBrandId, string? Model, int Limit)
        {
            var cars = await _context.Cars.Include(c => c.CarBrand).Include(p => p.CarPictures).ToListAsync();
            if (ModelState.IsValid)
            {

                // Apply filters based on selected options
                if (CarBrandId > 0)
                {
                    cars = cars.Where(c => c.CarBrand.Id == CarBrandId).ToList();
                }

                if (!string.IsNullOrEmpty(Model))
                {
                    cars = cars.Where(c => c.Model.Contains(Model)).ToList();
                }

                if (Limit > 0)
                    cars = cars.Take(Limit).ToList();

                ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name");
            }
            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarBrand)
                .Include(p => p.CarPictures)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CarBrands"] = new SelectList( _context.CarBrands.ToList(), "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CarBrandId,Model,Year,PassengerSeats,Description,RentalPricePerDay")] Car car ,IFormFile[] pictures, string newCarBrand)
        {
            try
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        string[] invalidFields = {"Year","Model","PassengerSeats","RentalPricePerDay"}; 
                        foreach (string er in invalidFields)
                        {
                            if (state.Key == er)
                            {

                                ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name");
                                return View(car);
                            }
                        }
                    }
                }
                    if (!string.IsNullOrEmpty(newCarBrand))
                    {
                        // Add new car brand if it doesn't exist
                        var existingBrand = await _context.CarBrands
                            .FirstOrDefaultAsync(b => b.Name.ToLower() == newCarBrand.ToLower());

                        if (existingBrand == null)
                        {
                            var carBrand = new CarBrand { Name = newCarBrand };
                            _context.CarBrands.Add(carBrand);
                            await _context.SaveChangesAsync();
                            car.CarBrandId = carBrand.Id; // Associate the new car brand with the car
                        }
                    }
                    _context.Add(car);
                    await _context.SaveChangesAsync();
                    TempData["FailMessage"] = $"Car has added";
                    await AddPictureAsync(pictures, car.Id);
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex) { }
            ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name");
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var car = await _context.Cars
                .Include(c => c.CarBrand)
                .Include(p => p.CarPictures)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["CarBrands"] = new SelectList(await _context.CarBrands.ToListAsync(), "Id", "Name");
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit([Bind("Id,CarBrandId,Model,Year,PassengerSeats,Description,RentalPricePerDay")] Car car, IFormFile[] pictures, string newCarBrand)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    string[] invalidFields = { "Year", "Model", "PassengerSeats", "RentalPricePerDay" };
                    foreach (string er in invalidFields)
                    {
                        if (state.Key == er)
                        {

                            ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name");
                            return View(car);
                        }
                    }
                }
            }
            if (car == null)
            {
                return NotFound();
            }
            
                try
                {
                    if (!string.IsNullOrEmpty(newCarBrand))
                    {
                        // Add new car brand if it doesn't exist
                        var existingBrand = await _context.CarBrands
                            .FirstOrDefaultAsync(b => b.Name.ToLower() == newCarBrand.ToLower());

                        if (existingBrand == null)
                        {
                            var carBrand = new CarBrand { Name = newCarBrand };
                            _context.CarBrands.Add(carBrand);
                            await _context.SaveChangesAsync();
                            car.CarBrandId = carBrand.Id; // Associate the new car brand with the car
                        }
                    }
                    _context.Update(car);
                    await _context.SaveChangesAsync();
				TempData["FailMessage"] = $"Car has edited removed";
				await AddPictureAsync(pictures, car.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarBrand)
                .Include(p => p.CarPictures)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.
                Include(p => p.CarPictures)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (await _context.Reservations.FirstOrDefaultAsync(k => k.Car.Id == id && k.IsApproved == true && k.EndDate > DateTime.Now.Date) is null)
            {
                foreach (var picture in car.CarPictures)
                {
                    await DeletePictureAsync(picture.Id, id);
                }
                if (car != null)
                {
                    _context.Cars.Remove(car);
                }
				TempData["FailMessage"] = $"Car has been removed";
			}
            else
            {
				TempData["FailMessage"] = $"Car has aproved reservation/s that haven't ended, so wansn't removed";
			}
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeletePictureAsync(int pictureId, int carId)
        {
            var picture = await _context.CarPictures.FindAsync(pictureId);
            if (picture != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", picture.PictureUrl.TrimStart('/'));

                // Delete the file if it exists
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                 _context.CarPictures.Remove(picture);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = carId });
        }
        public async Task AddPictureAsync(IFormFile[] pictures, int carId)
        {
            if(pictures != null && pictures.Length > 0)
            {
                // Folder to save the uploaded files (now 'uploads' instead of 'images')
                var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadDir);  // Ensure directory exists

                foreach (var picture in pictures)
                {
                    if (picture.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(picture.FileName)}";
                        var filePath = Path.Combine(uploadDir, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(stream);
                        }

                        // Save file URL to the database (Correct URL as "/uploads/")
                        var fileUrl = "/uploads/" + fileName;  // Update to use /uploads/
                        
                            var pictureEntity = new CarPicture
                            {
                                PictureUrl = fileUrl,
                                CarId = carId
                            };
                            _context.CarPictures.Add(pictureEntity);
                        
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
