using HotelListing.DTOS.Hotel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOS.Country
{
  public class UpdCountryDTO
  {
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too Long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 2, ErrorMessage = "Short Country Name is too Long")]
    public string ShortName { get; set; }

    public IList<AddHotelDTO> Hotels { get; set; }
  }
}
