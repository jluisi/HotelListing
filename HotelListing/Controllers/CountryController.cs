using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOS.Country;
using HotelListing.DTOS.Params;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
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

    public async Task<IActionResult> GetCountries([FromQuery] PagingDTO pagingDTO)
    {
      var countries = await _unitOfWork.Countries.GetPagedList(pagingDTO);
      var results = _mapper.Map<IList<CountryDTO>>(countries);  // map to List<CountryDTO>

      return Ok(results);
    }

    //---------------------------------------------------------------------------------------------
    [HttpGet("{id:int}", Name = "GetCountry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetCountry(int id)
    {
      var country = await _unitOfWork.Countries.Get(x => x.Id == id, new List<string> { "Hotels" });
      var result = _mapper.Map<CountryDTO>(country);
      return Ok(result);
    }

    //---------------------------------------------------------------------------------------------
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> CreateCountry([FromBody] AddCountryDTO countryDTO)
    {
      if (!ModelState.IsValid)
      {
        _logger.LogError($"Invalid INSERT Attempt in {nameof(CreateCountry)}");
        return BadRequest(ModelState);
      }

      // After Saving to the DB, the Entity will have the Id value of the new record
      var country = _mapper.Map<Country>(countryDTO);
      await _unitOfWork.Countries.Insert(country);
      await _unitOfWork.Save();

      // Returns whatever the GetCountry route returns, in this case the new Country
      return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
    }

    //---------------------------------------------------------------------------------------------
    // HttpPut always replaces all the properties, except the Id. For example, if a property is 
    // NOT required, and we don't supply a value, the original value (in DB) will be set to NULL
    //---------------------------------------------------------------------------------------------
    [Authorize]
    [HttpPut("{id:int}", Name = "UpdateCountry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdCountryDTO countryDTO)
    {
      if (!ModelState.IsValid || id < 1)
      {
        _logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateCountry)}");
        return BadRequest(ModelState);
      }

      // Get the record to update from the database
      var country = await _unitOfWork.Countries.Get(x => x.Id == id);

      // If the record does not exist in the database
      if (country == null)
      {
        _logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateCountry)}");
        return BadRequest("Submitted Data is Invalid, Record does NOT Exist");
      }

      // If the record exists
      _mapper.Map(countryDTO, country);
      _unitOfWork.Countries.Update(country);
      await _unitOfWork.Save();

      return NoContent();
    }

    //---------------------------------------------------------------------------------------------
    // Notice that the Country table was created with the Referential Integrity of Delete Cascade
    // Thus, when a Country is Deleted, all its Hotels will be deleted too. To avoid this behavior 
    // we need to change this constraint to Restrict: No Country can be deleted if it has Hotels
    //---------------------------------------------------------------------------------------------
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}", Name = "DeleteCountry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> DeleteCountry(int id)
    {
      if (id < 1)
      {
        _logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteCountry)}");
        return BadRequest();
      }

      var country = await _unitOfWork.Countries.Get(x => x.Id == id);

      if (country == null)
      {
        _logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteCountry)}");
        return BadRequest("Submitted Data is Invalid, Record does NOT Exist");
      }

      await _unitOfWork.Countries.Delete(country.Id);
      await _unitOfWork.Save();

      return NoContent();
    }

  }
}
