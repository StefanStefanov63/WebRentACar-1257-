using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebRentACar.Data;
using WebRentACar.Models;

namespace WebRentACar.Controllers
{
    public class CarBrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarBrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarBrands
        public async Task<IActionResult> Index()
        {
            return View(await _context.CarBrands.ToListAsync());
        }

        // GET: CarBrands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = await _context.CarBrands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carBrand == null)
            {
                return NotFound();
            }

            return View(carBrand);
        }

        // GET: CarBrands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarBrands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CarBrand carBrand)
        {
                _context.Add(carBrand);
                await _context.SaveChangesAsync();
			TempData["SuccessMessage"] = $"Added Brand.";
			return RedirectToAction(nameof(Index));
        }

        // GET: CarBrands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = await _context.CarBrands.FindAsync(id);
            if (carBrand == null)
            {
                return NotFound();
            }
            return View(carBrand);
        }

        // POST: CarBrands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CarBrand carBrand)
        {
            if (id != carBrand.Id)
            {
                return NotFound();
            }
            if (carBrand.Name.Length
                 <= 100)
            {
                try
                {
                    var brand = _context.CarBrands.FirstOrDefault(x => x.Id == id);
                    brand.Name = carBrand.Name;
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarBrandExists(carBrand.Id))
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
            return View(carBrand);
        }

        // GET: CarBrands/Delete/5
        public async Task<IActionResult> Delete()
        {

            var carBrands = await _context.CarBrands.ToListAsync();
            if (carBrands == null)
            {
                return NotFound();
            }

            return View(carBrands);
        }

        // POST: CarBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carBrand = await _context.CarBrands.FindAsync(id);
            if (carBrand != null)
            {
                var n = await _context.Cars.Where(x => x.Id == id).ToListAsync();
				if (n.IsNullOrEmpty())
                _context.CarBrands.Remove(carBrand);
				else
                {
					TempData["SuccessMessage"] = $"There are cars with this brand, so you can't remove it.";
					return View(await _context.CarBrands.ToListAsync());
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarBrandExists(int id)
        {
            return _context.CarBrands.Any(e => e.Id == id);
        }
    }
}
