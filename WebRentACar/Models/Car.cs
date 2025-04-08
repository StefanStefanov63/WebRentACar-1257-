using System.ComponentModel.DataAnnotations;

namespace WebRentACar.Models
{
	public class Car
	{
		public int Id { get; set; } 

		[Required]
		public int CarBrandId { get; set; } 

		public CarBrand CarBrand { get; set; } 

		[Required]
		[StringLength(100, ErrorMessage = "The model name cannot be longer than 100 characters.")]
		public string Model { get; set; } 

		[Required]
		[Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
		public int Year { get; set; } 

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "The number of seat/s must be at least 1 .")]
		public int PassengerSeats { get; set; } 

		[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
		public string Description { get; set; }

		[Required]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0(free).")]
		public double RentalPricePerDay { get; set; } 
		public ICollection<CarPicture> CarPictures { get; set; }
		public ICollection<Reservation> Reservations { get; set; }
	}
}
