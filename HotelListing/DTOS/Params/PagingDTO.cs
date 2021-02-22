using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.DTOS.Params
{
  public class PagingDTO
  {
    const int _maxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
      get { return _pageSize; }
      set { _pageSize = (value > _maxPageSize) ? _maxPageSize : value; }
    }
  }
}
