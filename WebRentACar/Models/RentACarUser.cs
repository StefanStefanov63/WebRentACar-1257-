using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebRentACar.Models
{
	public class RentACarUser : IdentityUser
	{

		[Required]
		public string FirstName { get; set; }

		[Required]
		public  string LastName { get; set; }

		[Required]
		[StringLength(10, MinimumLength = 10, ErrorMessage = "EGN must be exactly 10 digits.")]
		public string EGN { get; set; }

		//[Phone(ErrorMessage = "Invalid phone number format.")]
		//public string PhoneNumber { get; set; }

		//[EmailAddress(ErrorMessage = "Invalid email format.")]
		//public string Email { get; set; }
		public ICollection<Reservation> Reservations { get; set; }


    }
}
