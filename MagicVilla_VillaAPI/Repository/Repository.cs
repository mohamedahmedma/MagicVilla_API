using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> Set { get; set; }

        public Repository(AppDbContext appDbContext)
        {
            _context = appDbContext;
            Set = _context.Set<T>();
        }
        public async Task CreateAsync(T obj)
        {
            await Set.AddAsync(obj);
            await SaveAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null , string? includePro = null
            , int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = Set;

            if (filter != null)
            {
                query.Where(filter);
            }
            if(pageSize > 0)
            {
                if(pageSize > 100)
                {
                    pageSize = 100;
                }
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            if (includePro != null)
            {
                foreach (var includeProp in includePro.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
                return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true , string? includes = null)
        {
            IQueryable<T> query = Set;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includes != null)
            {
                foreach (var item in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }

            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(T obj)
        {
            Set.Remove(obj);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
