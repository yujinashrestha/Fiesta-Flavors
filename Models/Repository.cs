
using Microsoft.EntityFrameworkCore;


using Fiesta_Flavors.Data;

namespace Fiesta_Flavors.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected MyAppDbContext _context { get; set; }
        private DbSet<T> dbSet { get; set; }

        public Repository(MyAppDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }
        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public Task<T> GetIdAsync(int id, QueryOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
