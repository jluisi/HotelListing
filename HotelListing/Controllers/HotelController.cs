using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOS.Hotel;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class HotelController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelController> _logger;
    private readonly IMapper _mapper;

    //---------------------------------------------------------------------------------------------
    public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _mapper = mapper;
    }

    //---------------------------------------------------------------------------------------------
    [HttpGet(Name = "GetHotels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetHotels()
    {
      try
      {
        var hotels = await _unitOfWork.Hotels.GetAll();
        var results = _mapper.Map<IList<HotelDTO>>(hotels);
        return Ok(results);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

    //---------------------------------------------------------------------------------------------
    [HttpGet("{id:int}", Name = "GetHotel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetHotel(int id)
    {
      try
      {
        var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id, new List<string> { "Country" });
        var result = _mapper.Map<HotelDTO>(hotel);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotel)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

    //---------------------------------------------------------------------------------------------
    [Authorize(Roles = "Admin")]
    [HttpPost(Name = "CreateHotel")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> CreateHotel([FromBody] AddHotelDTO hotelDTO)
    {
      if (!ModelState.IsValid)
      {
        _logger.LogError($"Invalid INSERT Attempt in {nameof(CreateHotel)}");
        return BadRequest(ModelState);
      }

      try
      {
        // After Saving to the DB, the Entity will have the Id value of the new record
        var hotel = _mapper.Map<Hotel>(hotelDTO);
        await _unitOfWork.Hotels.Insert(hotel);
        await _unitOfWork.Save();

        // Returns whatever the GetHotel route returns, in this case the new Hotel
        return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(CreateHotel)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

    //---------------------------------------------------------------------------------------------
    // HttpPut always replaces all the properties, except the Id. For example, if a property is 
    // NOT required, and we don't supply a value, the original value (in DB) will be set to NULL
    //---------------------------------------------------------------------------------------------
    [Authorize]
    [HttpPut("{id:int}", Name = "UpdateHotel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdHotelDTO hotelDTO)
    {
      if (!ModelState.IsValid || id < 1)
      {
        _logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateHotel)}");
        return BadRequest(ModelState);
      }

      try
      {
        // Get the record to update from the database
        var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);

        // If the record does not exist in the database
        if (hotel == null)
        {
          _logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateHotel)}");
          return BadRequest("Submitted Data is Invalid, Record does NOT Exist");
        }

        // If the record exists
        _mapper.Map(hotelDTO, hotel);
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.Save();

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateHotel)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

    //---------------------------------------------------------------------------------------------
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}", Name = "DeleteHotel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> DeleteHotel(int id)
    {
      if (id < 1)
      {
        _logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteHotel)}");
        return BadRequest();
      }

      try
      {
        var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);

        if (hotel == null)
        {
          _logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteHotel)}");
          return BadRequest("Submitted Data is Invalid, Record does NOT Exist");
        }

        await _unitOfWork.Hotels.Delete(hotel.Id);
        await _unitOfWork.Save();

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteHotel)}");
        return StatusCode(500, "Internal Server Error. Please try again.");
      }
    }

  }
}
