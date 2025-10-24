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

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T? entity = await dbSet.FindAsync(id);  // Add ? to make nullable

            if (entity != null)  // Check for null before removing
            {
                dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
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

        public async Task UpdateAsync(T entity)
        {
             _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
        {
            IQueryable<T> query = dbSet;
            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }

            if (options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }

            foreach(string include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            //Filter by the specifies property name and id

            query = query.Where(e => EF.Property<TKey>(e, propertyName).Equals(id));

            return await query.ToListAsync();

        }
    }
}