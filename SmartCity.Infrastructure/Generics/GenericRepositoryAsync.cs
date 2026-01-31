using Microsoft.EntityFrameworkCore;
using SmartCity.Infrastructure.Data;

namespace SmartCity.Infrastructure.Generics
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        #region Fields

        protected readonly ApplicationDbContext context;

        #endregion

        #region Constructor
        public GenericRepositoryAsync(ApplicationDbContext _context)
        {
            context = _context;
        }

        #endregion

        #region Handle Functions

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();

            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public IQueryable<T> GetTableNoTracking()
        {
            return context.Set<T>().AsNoTracking().AsQueryable();
        }

        public IQueryable<T> GetTableAsTracking()
        {
            return context.Set<T>().AsQueryable();

        }

        #endregion
    }
}
