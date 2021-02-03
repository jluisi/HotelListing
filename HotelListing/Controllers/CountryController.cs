using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.DTOS.Country;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountryController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    //---------------------------------------------------------------------------------------------
    // Constructor: unitOfWork, logger and mapper are injected
    //---------------------------------------------------------------------------------------------
    public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, 
      IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _mapper = mapper;
    }

    //---------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetCountries()
    {
      try
      {
        var countries = await _unitOfWork.Countries.GetAll();
        var results = _mapper.Map<IList<CountryDTO>>(countries);  // map to List<CountryDTO>
        return Ok(results);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

    //---------------------------------------------------------------------------------------------
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetCountry(int id)
    {
      try
      {
        var country = await _unitOfWork.Countries.Get(x => x.Id == id, new List<string> { "Hotels" });
        var result = _mapper.Map<CountryDTO>(country); 
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountry)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

  }
}
