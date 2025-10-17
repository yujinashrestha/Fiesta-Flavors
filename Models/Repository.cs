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

        public async Task<T?> GetIdAsync(int id, QueryOptions<T>? options)
        {
            IQueryable<T> query = dbSet;

            if (options != null)
            {
                if (options.HasWhere)
                {
                    query = query.Where(options.Where);
                }

                if (options.HasOrderBy)
                {
                    query = query.OrderBy(options.OrderBy);
                }

                foreach (string include in options.GetIncludes())
                {
                    query = query.Include(include);
                }
            }

            var entityType = _context.Model.FindEntityType(typeof(T));
            if (entityType == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
            }

            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey == null)
            {
                throw new InvalidOperationException($"No primary key defined for entity type {typeof(T).Name}.");
            }

            var keyProperty = primaryKey.Properties.FirstOrDefault();
            if (keyProperty == null)
            {
                throw new InvalidOperationException($"Primary key property not found for entity type {typeof(T).Name}.");
            }

            string primaryKeyName = keyProperty.Name;

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}