using System.Collections.Generic;
using System.Threading.Tasks;
using Artsofte.Db;
using Microsoft.EntityFrameworkCore;

namespace Artsofte.Repositories
{
    public abstract class AbstractRepository<T>
        where T : class
    {
        protected readonly StaffContext StaffContext;

        public AbstractRepository(StaffContext staffContext)
        {
            StaffContext = staffContext;
        }

        public virtual async Task<T> AddAsync(T model)
        {
            await StaffContext.AddAsync(model);
            await StaffContext.SaveChangesAsync();
            return model;
        }

        public virtual async Task<T> UpdateAsync(T model)
        {
            StaffContext.Update(model);
            await StaffContext.SaveChangesAsync();
            return model;
        }

        public virtual async Task DeleteAsync(T model)
        {
            StaffContext.Remove(model);
            await StaffContext.SaveChangesAsync();
        }

        public virtual async Task<T> FindAsync(int id)
        {
            return await StaffContext
                .Set<T>()
                .FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await StaffContext
                .Set<T>()
                .ToListAsync();
        }
    }
}