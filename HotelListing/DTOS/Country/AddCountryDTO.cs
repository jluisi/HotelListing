using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOS.Country
{
  public class AddCountryDTO
  {
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too Long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 2, ErrorMessage = "Short Country Name is too Long")]
    public string ShortName { get; set; }
  }
}
