using System;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOS.Hotel
{
  public class UpdHotelDTO
  {
    [Required]
    [StringLength(maximumLength: 150, ErrorMessage = "Hotel Name is too Long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 250, ErrorMessage = "Hotel Address is too Long")]
    public string Address { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Ranking is Out of Valid Range")]
    public double Rating { get; set; }

    [Required]
    public int CountryId { get; set; }
  }
}
