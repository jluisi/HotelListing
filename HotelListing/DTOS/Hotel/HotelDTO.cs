using HotelListing.DTOS.Country;

namespace HotelListing.DTOS.Hotel
{
  public class HotelDTO : AddHotelDTO
  {
    public int Id { get; set; }

    public CountryDTO Country { get; set; }
  }
}
