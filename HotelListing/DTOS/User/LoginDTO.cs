using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOS.User
{
  public class LoginDTO
  {
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "{0} is between {2} to {1} characters")]
    public string Password { get; set; }
  }
}
