using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null ,string? includePro = null , 
            int pageSize = 0 , int pageNumber = 1);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true , string? includes = null);
        Task CreateAsync(T obj);
        Task RemoveAsync(T obj);
        Task SaveAsync();

    }
}
