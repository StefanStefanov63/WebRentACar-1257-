using Microsoft.AspNetCore.Identity;
using WebRentACar.Models;

namespace WebRentACar.Data
{
    public static class SeedData
    {
        public static async Task SeedingDbUsersAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<RentACarUser>>();
                var context = services.GetRequiredService<ApplicationDbContext>();

                var roles = new string[] { "Admin", "User" };
                foreach (var role in roles)
                {
                    var roleExist = await roleManager.RoleExistsAsync(role);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                context.Database.EnsureCreated();
                if (!context.Users.Any())
                {
                    var adminUser = await userManager.FindByEmailAsync("admin@car.com");
                    if (adminUser == null)
                    {
                        adminUser = new RentACarUser
                        {
                            UserName = "admin@car.com",
                            Email = "admin@car.com",
                            FirstName = "Admin",
                            LastName = "User",
                            EGN = "1234567890",
                            PhoneNumber = "123-456-7890"
                        };
                        var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                        }
                    }

                    var regularUser = await userManager.FindByEmailAsync("user@car.com");
                    if (regularUser == null)
                    {
                        regularUser = new RentACarUser
                        {
                            UserName = "user@car.com",
                            Email = "user@car.com",
                            FirstName = "User",
                            LastName = "Usero",
                            EGN = "1234537840",
                            PhoneNumber = "123-456-7830"
                        };
                        var result = await userManager.CreateAsync(regularUser, "UserPassword123!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(regularUser, "User");
                        }
                    }
                }
                

                if (!context.CarBrands.Any())
                {
                    context.CarBrands.AddRange(
                        new CarBrand { Name = "Toyota" },
                        new CarBrand { Name = "Ford" },
                        new CarBrand { Name = "BMW" },
                        new CarBrand { Name = "Audi" }
                    );
                    context.SaveChanges();
                }

                if (!context.Cars.Any())
                {
                    context.Cars.AddRange(
                        new Car
                        {
                            CarBrandId = 1,  // Toyota
                            Model = "Corolla",
                            Year = 2020,
                            PassengerSeats = 5,
                            Description = "Reliable and fuel-efficient sedan.",
                            RentalPricePerDay = 30.00
                        },
                        new Car
                        {
                            CarBrandId = 2,  // Ford
                            Model = "Mustang",
                            Year = 2021,
                            PassengerSeats = 4,
                            Description = "Sporty coupe with powerful performance.",
                            RentalPricePerDay = 70.00
                        },
                        new Car
                        {
                            CarBrandId = 3,  // BMW
                            Model = "X5",
                            Year = 2022,
                            PassengerSeats = 5,
                            Description = "Luxurious and spacious SUV.",
                            RentalPricePerDay = 120.00
                        },
                        new Car
                        {
                            CarBrandId = 4,  // Audi
                            Model = "A6",
                            Year = 2021,
                            PassengerSeats = 5,
                            Description = "Elegant sedan with advanced features.",
                            RentalPricePerDay = 100.00
                        }
                    );
                    context.SaveChanges();
                }
                if (!context.CarPictures.Any())
                {
                    context.CarPictures.AddRange(
                        new CarPicture { PictureUrl = "/uploads/98e4ed97-75d9-4af7-ae9e-237cd258ed18_wp8456061-881238823.jpg", CarId = 4 },
                        new CarPicture { PictureUrl = "/uploads/15c806cd-90d7-4983-8430-41217c14a545_2021-audi-a6_100747118_h-4198157168.jpg", CarId = 4 },
                        new CarPicture { PictureUrl = "/uploads/250e74df-0635-4ea0-a423-aa6127a74a7a_2021-Ford-Mustang-Mach-1-0-590574180.jpg", CarId = 2 }
                    );
                    context.SaveChanges();
                }
                if (!context.Reservations.Any())
                {
                    context.Reservations.AddRange(
                        new Reservation
                        {
                            CarId = 4,
                            UserId = context.Users.FirstOrDefault(x => x.UserName == "admin@car.com").Id,
                            StartDate = new DateTime(2025, 4, 12),
                            EndDate = new DateTime(2025, 5, 12),
                            IsApproved = true
                        },
                        new Reservation
                        {
                            CarId = 4,
                            UserId = context.Users.FirstOrDefault(x => x.UserName == "admin@car.com").Id,
                            StartDate = new DateTime(2025, 4, 1),
                            EndDate = new DateTime(2025, 4, 11),
                            IsApproved = false
                        },
                        new Reservation
                        {
                            CarId = 2,
                            UserId = context.Users.FirstOrDefault(x => x.UserName == "user@car.com").Id,
                            StartDate = new DateTime(2025, 4, 1),
                            EndDate = new DateTime(2025, 4, 4),
                            IsApproved = true
                        },
                        new Reservation
                        {
                            CarId = 2,
                            UserId = context.Users.FirstOrDefault(x => x.UserName == "user@car.com").Id,
                            StartDate = new DateTime(2025, 4, 6),
                            EndDate = new DateTime(2025, 4, 27),
                            IsApproved = false
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
