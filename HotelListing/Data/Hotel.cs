﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.Data
{
  public class Hotel
  {
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Address { get; set; }

    public double Rating { get; set; }

    [ForeignKey("Country")]
    public int CountryId { get; set; }
    public Country Country { get; set; }
  }
}
