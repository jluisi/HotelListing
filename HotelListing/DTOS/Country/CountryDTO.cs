using HotelListing.DTOS.Hotel;
using System.Collections.Generic;

namespace HotelListing.DTOS.Country
{
  public class CountryDTO : AddCountryDTO
  {
    public int Id { get; set; }

    public IList<HotelDTO> Hotels { get; set; }
  }
}
