using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOS.Country;
using HotelListing.DTOS.Hotel;

namespace HotelListing.Configurations
{
  public class MapperInitializer : Profile
  {
    public MapperInitializer()
    {
      CreateMap<Country, CountryDTO>().ReverseMap();
      CreateMap<Country, AddCountryDTO>().ReverseMap();

      CreateMap<Hotel, HotelDTO>().ReverseMap();
      CreateMap<Hotel, AddHotelDTO>().ReverseMap();
    }
  }
}
