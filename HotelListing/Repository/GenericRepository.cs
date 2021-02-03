using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
  public class GenericRepository<T> : IGenericRepository<T> where T : class
  {
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    //---------------------------------------------------------------------------------------------
    // Constructor receiving the Application Context via Dependency Injection
    //---------------------------------------------------------------------------------------------
    public GenericRepository(AppDbContext context)
    {
      _context = context;
      _dbSet = _context.Set<T>();   // returns a DbSet of <T>
    }

    //---------------------------------------------------------------------------------------------
    public async Task Delete(int id)
    {
      var entity = await _dbSet.FindAsync(id);
      if (entity != null)
      {
        _dbSet.Remove(entity);
      }
    }

    //---------------------------------------------------------------------------------------------
    public void DeleteRange(IEnumerable<T> entities)
    {
      _dbSet.RemoveRange(entities);
    }

    //---------------------------------------------------------------------------------------------
    public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
    {
      IQueryable<T> query = _dbSet;

      // Includes objects attached to this entity
      if (includes != null)
      {
        foreach (var includeProperty in includes)
        {
          query = query.Include(includeProperty);
        }
      }

      return await query.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    //---------------------------------------------------------------------------------------------
    public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
    {
      IQueryable<T> query = _dbSet;

      // Check whether there is a lambda expression that will filter the data
      if (expression != null)
      {
        query = query.Where(expression);
      }

      // Includes objects attached to this entity
      if (includes != null)
      {
        foreach (var includeProperty in includes)
        {
          query = query.Include(includeProperty);
        }
      }

      // Sort the returning List
      if (orderBy != null)
      {
        query = orderBy(query);
      }

      return await query.AsNoTracking().ToListAsync();
    }

    //---------------------------------------------------------------------------------------------
    public async Task Insert(T entity)
    {
      await _dbSet.AddAsync(entity);
    }

    //---------------------------------------------------------------------------------------------
    public async Task InsertRange(IEnumerable<T> entities)
    {
      await _dbSet.AddRangeAsync(entities);
    }

    //---------------------------------------------------------------------------------------------
    public void Update(T entity)
    {
      _dbSet.Attach(entity);
      _context.Entry(entity).State = EntityState.Modified;
    }
  }
}
