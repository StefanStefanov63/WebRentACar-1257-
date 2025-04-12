using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Data;
using WebRentACar.Models;

namespace WebRentACar.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> SeeMyReservations()
        {
            string UserId = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).Id;
            var applicationDbContext = _context.Reservations.Include(r => r.Car).Include(r => r.User).Include(r => r.Car.CarBrand).Where(r => r.UserId == UserId);
            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (reservation.IsApproved)
            {
                TempData["FailMessage"] = $"This Reservation is alredy approved";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                string conflict = await VerifyDateTimeAsync(reservation);
                if (conflict is null)
                {
                    TempData["FailMessage"] = $"Sucsefully aproved reservation";
                    reservation.IsApproved = true;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                    
                }
                else
                {
                    TempData["FailMessage"] = $"This car is alredy reserved from " + conflict.ToString();
                    
                }
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(SeeMyReservations));

            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (!reservation.IsApproved)
            {
                TempData["FailMessage"] = $"This Reservation is alredy unapproved";
            }
            else
            {
                    TempData["FailMessage"] = $"Sucsefully unaproved reservation";
                    reservation.IsApproved = false;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
            }
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(SeeMyReservations));
        }
        // GET: Reservations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reservations.Include(r => r.Car).Include(r => r.User).Include(r => r.Car.CarBrand);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int Carid)
        {
            ViewData["CarId"] = Carid;
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Carid,DateTime StartDate, DateTime EndDate)
        {

            Reservation reservation = new Reservation
                {
                    UserId = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).Id,
                    CarId = Carid,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    IsApproved = false
                };
            if (!reservation.IsValidReservation()) 
            {

                TempData["FailMessage"] = $"End Date should be after StartDate";
                ViewData["CarId"] = Carid;
                return View(reservation);
            }
            string conflict = await VerifyDateTimeAsync(reservation);
            if (conflict is null)
            {
                TempData["FailMessage"] = $"Sucsefully added reservation";
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(SeeMyReservations));
            }
            else
            {
                TempData["FailMessage"] = $"This car is alredy reserved from " + conflict.ToString();
                ViewData["CarId"] = Carid;
                return View(reservation);
            }
            
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = reservation.CarId;
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Carid, DateTime StartDate, DateTime EndDate)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if(reservation == null)
                return NotFound();
            reservation.StartDate = StartDate;
            reservation.EndDate = EndDate;
            if (!reservation.IsValidReservation())
            {

                TempData["FailMessage"] = $"End Date should be after StartDate";
                ViewData["CarId"] = Carid;
                return View(reservation);
            }
            if (!reservation.IsValidReservation())
            {

                TempData["FailMessage"] = $"End Date should be after StartDate";
                return View(reservation);
            }
            string conflict = await VerifyDateTimeAsync(reservation);
            if (conflict is null)
            {
                TempData["FailMessage"] = $"Sucsefully edited reservation";
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(SeeMyReservations));
            }
            else
            {
                TempData["FailMessage"] = $"This car is alredy reserved from " + conflict.ToString();
                ViewData["CarId"] = Carid;
                return View(reservation);
            }
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }
            TempData["FailMessage"] = $"Sucsefully deleted reservation";
            await _context.SaveChangesAsync();
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(SeeMyReservations));
        }
        public async Task<string?> VerifyDateTimeAsync(Reservation res)
        {
            var existingReservations = await _context.Reservations.Where(x => x.CarId == res.CarId && x.IsApproved == true).ToListAsync();
            
            foreach (var existingRes in existingReservations)
            {
                if (res.StartDate <= existingRes.EndDate && res.EndDate >= existingRes.StartDate) 
                {
                    string conflict = existingRes.StartDate.ToString() + " to " +existingRes.EndDate.ToString();
                    return conflict;
                }
            }
            return null;
        }

            private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
