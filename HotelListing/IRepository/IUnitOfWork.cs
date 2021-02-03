using HotelListing.Data;
using System;
using System.Threading.Tasks;

namespace HotelListing.IRepository
{
  //---------------------------------------------------------------------------------------------
  // Acts like a register for every variation of the generic repository 
  //---------------------------------------------------------------------------------------------
  public interface IUnitOfWork : IDisposable
  {
    IGenericRepository<Country> Countries { get; }
    
    IGenericRepository<Hotel> Hotels { get; }

    // Save all changes to the database in one call
    Task Save();
  }
}
