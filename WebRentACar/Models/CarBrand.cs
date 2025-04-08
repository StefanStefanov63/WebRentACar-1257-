using System.ComponentModel.DataAnnotations;

namespace WebRentACar.Models
{
	public class CarBrand
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }
		public ICollection<Car> Cars { get; set; }
	}
}
