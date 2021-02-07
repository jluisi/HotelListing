using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOS.Country;
using HotelListing.DTOS.Hotel;
using HotelListing.DTOS.User;

namespace HotelListing.Configurations
{
  public class MapperInitializer : Profile
  {
    public MapperInitializer()
    {
      CreateMap<Country, CountryDTO>().ReverseMap();
      CreateMap<Country, AddCountryDTO>().ReverseMap();
      CreateMap<Country, UpdCountryDTO>().ReverseMap();

      CreateMap<Hotel, HotelDTO>().ReverseMap();
      CreateMap<Hotel, AddHotelDTO>().ReverseMap();
      CreateMap<Hotel, UpdHotelDTO>().ReverseMap();

      CreateMap<ApiUser, UserDTO>().ReverseMap();
    }
  }
}
