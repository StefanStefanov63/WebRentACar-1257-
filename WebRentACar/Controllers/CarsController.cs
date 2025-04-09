using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cars.Include(c => c.CarBrand);
            return View(await applicationDbContext.ToListAsync());
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
        public async Task<IActionResult> Create(int CarBrandId,string Model,int Year,int PassengerSeats,string Description,double RentalPricePerDay, IFormFile[] pictures, string newCarBrand)
        {
            Car car = new Car
            {
                CarBrandId = CarBrandId,
                Model = Model,
                Year = Year,
                PassengerSeats = PassengerSeats,
                Description = Description,
                RentalPricePerDay = RentalPricePerDay
            };
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
            //
            TempData["SuccessMessage"] = "Car added successfully!";
            if (pictures != null && pictures.Length > 0)
            {
                // Folder to save the uploaded files (now 'uploads' instead of 'images')
                var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadDir);  // Ensure directory exists

                foreach (var picture in pictures)
                {
                    if (picture.Length > 0)
                    {
                        var fileName = Path.GetFileName(picture.FileName);
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
                            CarId = car.Id
                        };
                        _context.CarPictures.Add(pictureEntity);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Handle picture uploads if any
     //       if (pictures != null)
     //           {
     //           string folderPath = "images/";
     //           foreach (var file in pictures)
     //               {
     //               //if (!file.ContentType.StartsWith("image/"))
     //               //{

     //               folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

     //               string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

     //               await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

     //               folderPath = "/" + folderPath;
     //               //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
     //               //        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");

     //               //        if (!Directory.Exists(uploadPath))
     //               //        {
     //               //            Directory.CreateDirectory(uploadPath);
     //               //        }

     //               //        var filePath = Path.Combine(uploadPath, fileName);
     //               //        using (var stream = new FileStream(filePath, FileMode.Create))
     //               //        {
     //               //            await file.CopyToAsync(stream);
     //               //        }
					
					//var carPicture = new CarPicture
     //                   {
     //                       PictureUrl = folderPath,
     //                       CarId = car.Id
					//		};

     //                       _context.CarPictures.Add(carPicture);
     //                   //}
     //               }

     //               await _context.SaveChangesAsync();
     //           }
            
            //ViewData["CarBrands"] = new SelectList(_context.CarBrands.ToList(), "Id", "Name", car.CarBrand);
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", car.CarBrandId);
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarBrandId,Model,Year,PassengerSeats,Description,RentalPricePerDay")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
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
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", car.CarBrandId);
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarBrand)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
