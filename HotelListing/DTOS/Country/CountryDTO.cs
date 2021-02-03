using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOS.Country
{
  public class CountryDTO : AddCountryDTO
  {
    public int Id { get; set; }

    public IList<HotelDto> Hotels { get; set; }
  }
}
