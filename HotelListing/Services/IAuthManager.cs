using HotelListing.DTOS.User;
using System.Threading.Tasks;

namespace HotelListing.Services
{
  public interface IAuthManager
  {
    Task<bool> ValidateUser(LoginDTO loginDTO);

    Task<string> CreateToken();
  }
}
