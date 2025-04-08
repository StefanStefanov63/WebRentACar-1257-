using System.ComponentModel.DataAnnotations;

namespace WebRentACar.Models
{
	public class Reservation
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int CarId { get; set; } 

		public Car Car { get; set; } 

		[Required]
		public string UserId { get; set; } 

		public RentACarUser User { get; set; } 

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; } 

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; } 

		public bool IsApproved { get; set; } 
		public bool IsValidReservation()
		{
			return EndDate > StartDate;
		}
	}
}
