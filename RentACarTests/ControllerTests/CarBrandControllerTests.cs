using Microsoft.EntityFrameworkCore;
using Moq;
using WebRentACar.Controllers;
using WebRentACar.Data;
using WebRentACar.Models;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Controllers;
using WebRentACar.Data;
using WebRentACar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Controllers;
using WebRentACar.Data;
using WebRentACar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Controllers;
using WebRentACar.Data;
using WebRentACar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Controllers;
using WebRentACar.Data;
using WebRentACar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebRentACar.Tests.Controllers
{
    public class CarBrandsControllerTests
    {
        private CarBrandsController _controller;
        private ApplicationDbContext _context;

        // Setup method to initialize context and controller
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")  // In-memory database name
                .Options;

            _context = new ApplicationDbContext(options);

            // Clear any existing data from previous tests
            _context.CarBrands.RemoveRange(_context.CarBrands);
            _context.Cars.RemoveRange(_context.Cars);  // Optional: if using the Car table in delete tests
            _context.SaveChanges();

            // Add test data with unique IDs for each test
            _context.CarBrands.AddRange(
                new CarBrand { Id = 1, Name = "Toyota" },
                new CarBrand { Id = 2, Name = "Honda" }
            );
            _context.SaveChanges();

            // Create the controller with the fresh context
            _controller = new CarBrandsController(_context);
        }


        // TearDown method to dispose of controller and context after each test
        [TearDown]
        public void TearDown()
        {
            // Dispose of context (since _controller directly depends on it)
            _context.CarBrands.RemoveRange(_context.CarBrands);
            _context.Cars.RemoveRange(_context.Cars);  // Optional: if using Car table in delete tests
            _context.SaveChanges();

            // Dispose of the controller
            _controller?.Dispose();  // Dispose of the controller if it’s not null

            // Dispose of the context
            _context?.Dispose();  // Dispose of the context if it’s not null
        }



        [Test]
        public async Task Index_ReturnsViewWithCarBrands()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var result = await _controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as List<CarBrand>;
            Assert.AreEqual(2, model.Count);
        }

        // Test for Details action (valid ID)
        [Test]
        public async Task Details_ValidId_ReturnsCorrectCarBrand()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var result = await _controller.Details(1) as ViewResult;

            Assert.IsNotNull(result);
            var carBrand = result.Model as CarBrand;
            Assert.AreEqual("Toyota", carBrand.Name);
        }

        // Test for Details action (invalid ID)
        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var result = await _controller.Details(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Test for Create action (GET)
        [Test]
        public void Create_ReturnsView()
        {
            var result = _controller.Create();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        // Test for Create action (POST - valid)
        [Test]
        public async Task Create_ValidModel_ReturnsRedirectToIndex()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var newBrand = new CarBrand { Name = "Ford" };
            var result = await _controller.Create(newBrand) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        // Test for Create action (POST - invalid)
        [Test]
        public async Task Create_InvalidModel_ReturnsView()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            _controller.ModelState.AddModelError("Name", "Name is required");
            var newBrand = new CarBrand { Name = "" };

            var result = await _controller.Create(newBrand) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(newBrand, result.Model);
        }

        // Test for Edit action (GET - valid ID)
        [Test]
        public async Task Edit_ValidId_ReturnsViewWithCarBrand()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var result = await _controller.Edit(1) as ViewResult;

            Assert.IsNotNull(result);
            var carBrand = result.Model as CarBrand;
            Assert.AreEqual("Toyota", carBrand.Name);
        }

        // Test for Edit action (GET - invalid ID)
        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var result = await _controller.Edit(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Test for Edit action (POST - valid)
        [Test]
        public async Task Edit_ValidModel_ReturnsRedirectToIndex()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var updatedBrand = new CarBrand { Id = 1, Name = "UpdatedToyota" };

            var result = await _controller.Edit(1, updatedBrand) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var brand = _context.CarBrands.FirstOrDefault(x => x.Id == 1);
            Assert.AreEqual("UpdatedToyota", brand.Name);
        }

        [Test]
        public async Task Edit_InvalidModel_ReturnsView()
        {
            // Clear any existing CarBrands
            _context.CarBrands.RemoveRange(_context.CarBrands);
            await _context.SaveChangesAsync();  // Save changes to ensure the table is empty

            // Add a valid CarBrand to edit
            var validBrand = new CarBrand { Id = 1, Name = "Toyota" };
            _context.CarBrands.Add(validBrand);
            await _context.SaveChangesAsync();

            // Create an invalid CarBrand (empty name)
            var updatedBrand = new CarBrand { Id = 1, Name = "" };  // Invalid because Name is empty

            // Simulate an invalid model state
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act: Call the Edit action (POST)
            var result = await _controller.Edit(1, updatedBrand) as ViewResult;

            // Assert: The result should not be null, and the model returned should be the same as the updatedBrand
            Assert.IsNotNull(result);
            var model = result.Model as CarBrand;
            Assert.AreEqual(updatedBrand.Id, model?.Id);  // Ensure the Id is correct
            Assert.AreEqual(updatedBrand.Name, model?.Name);  // Ensure the Name is the invalid one
        }

        // Test for Delete action (GET - valid ID)
        [Test]
        public async Task Delete_ValidId_ReturnsViewWithCarBrands()
        {
            var result = await _controller.Delete(1) as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as List<CarBrand>;
            Assert.AreEqual(2, model.Count);
        }

        // Test for Delete action (POST - valid)
        [Test]
        public async Task DeleteConfirmed_ValidId_ReturnsRedirectToIndex()
        {
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var brand = _context.CarBrands.FirstOrDefault(x => x.Id == 1);
            Assert.IsNull(brand);  // It should be deleted
        }

        // Test for Delete action (POST - cars associated with brand)
        [Test]
        public async Task DeleteConfirmed_WithCarsAssociated_ReturnsViewWithMessage()
        {
            _context.CarBrands.RemoveRange(_context.CarBrands);
            var brandWithCars = new CarBrand { Id = 3, Name = "Chevrolet" };
            _context.CarBrands.Add(brandWithCars);
            _context.Cars.Add(new Car { CarBrandId = 3 });
            _context.SaveChanges();

            var result = await _controller.DeleteConfirmed(3) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("There are cars with this brand, so you can't remove it.", _controller.TempData["SuccessMessage"]);
        }
    }
}



