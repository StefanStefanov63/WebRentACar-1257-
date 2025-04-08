using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRentACar.Models;

namespace WebRentACar.Data
{
	public class ApplicationDbContext : IdentityDbContext<RentACarUser>
    {
		public DbSet<Car> Cars { get; set; }
		public DbSet<CarBrand> CarBrands { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<CarPicture> CarPictures { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<RentACarUser>()
				.HasMany(r => r.Reservations)
				.WithOne(r => r.User)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Car>()
				.HasMany(c => c.CarPictures)
				.WithOne(cp => cp.Car)
				.HasForeignKey(cp => cp.CarId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CarBrand>()
				.HasMany(c => c.Cars)
				.WithOne(cp => cp.CarBrand)
				.HasForeignKey(cp => cp.CarBrandId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<RentACarUser>()
				.HasIndex(u => u.EGN)
				.IsUnique();

            modelBuilder.Entity<RentACarUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<RentACarUser>()
				.HasIndex(u => u.Email)
				.IsUnique();

            modelBuilder.Entity<RentACarUser>()
           .ToTable("AspNetUsers")
           .HasDiscriminator<string>("Discriminator")
           .HasValue<RentACarUser>("RentACarUser");
        }
		//public override async Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
		//{
		//	// Prevent deleting users with future approved reservations
		//	var usersToDelete = ChangeTracker.Entries<RentACarUser>()
		//		.Where(e => e.State == EntityState.Deleted)
		//		.Select(e => e.Entity)
		//		.ToList();

		//	foreach (var user in usersToDelete)
		//	{
		//		var userHasFutureApprovedReservations = await Reservations
		//			.Where(r => r.UserId == user.Id && r.IsApproved && r.EndDate > DateTime.Now)
		//			.AnyAsync();

		//		if (userHasFutureApprovedReservations)
		//		{
		//			throw new InvalidOperationException("User cannot be deleted because they have future approved reservations.");
		//		}
		//	}

		//	// Prevent deleting cars with future approved reservations
		//	var carsToDelete = ChangeTracker.Entries<Car>()
		//		.Where(e => e.State == EntityState.Deleted)
		//		.Select(e => e.Entity)
		//		.ToList();

		//	foreach (var car in carsToDelete)
		//	{
		//		var carHasFutureApprovedReservations = await Reservations
		//			.Where(r => r.CarId == car.Id && r.IsApproved && r.EndDate > DateTime.Now)
		//			.AnyAsync();

		//		if (carHasFutureApprovedReservations)
		//		{
		//			throw new InvalidOperationException("Car cannot be deleted because it has future approved reservations.");
		//		}
		//	}

		//	return await base.SaveChangesAsync(cancellationToken);
		//}
	}
}
