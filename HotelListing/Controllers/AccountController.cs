using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOS.User;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<ApiUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthManager _authManager;

    //---------------------------------------------------------------------------------------------
    public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, 
      IMapper mapper, IAuthManager authManager)
    {
      _userManager = userManager;
      _logger = logger;
      _mapper = mapper;
      _authManager = authManager;
    }

    //---------------------------------------------------------------------------------------------
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
      _logger.LogInformation($"Registration attempt for {userDTO.Email}");

      // IsValid property is true if the values of the data posted to the server were able
      // to bind correctly to the model and no validation rules were broken in the process
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        // Maps the object with posted data (userDTO) to the data entity object (ApiUser)
        var user = _mapper.Map<ApiUser>(userDTO);
        user.UserName = userDTO.Email;

        // Create the user in the database
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {
          foreach(var error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
          return BadRequest(ModelState);
        }

        await _userManager.AddToRolesAsync(user, userDTO.Roles);
        return Accepted();
      }
      catch (Exception ex)
      {
        // Using nameof, if the method name changes, we must change all of them or the code 
        // will not compile. The benefit is that we can detect these errors at compile time
        _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
        return Problem($"Login Access Denied in {nameof(Register)}", statusCode: 500);
      }
    }

    //---------------------------------------------------------------------------------------------
    [HttpPost]
    [Route("login")]
    
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
      _logger.LogInformation($"Login attempt for {loginDTO.Email}");

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        if (!await _authManager.ValidateUser(loginDTO))
        {
          return Unauthorized();
        }

        return Accepted(new { Token = await _authManager.CreateToken() });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
        return Problem($"Login Access Denied in {nameof(Login)}", statusCode: 500);
      }
    }

  }
}
