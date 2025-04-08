using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace WebRentACar.Models
{
	public class CarPicture
	{
		[Key]
		public int Id { get; set; } 

		[Required]
		[StringLength(500, ErrorMessage = "The picture URL cannot exceed 500 characters.")]
		[Url(ErrorMessage = "Invalid URL format.")]
		public string PictureUrl { get; set; } 

		public int CarId { get; set; } 

		public Car Car { get; set; } 
	}
}
