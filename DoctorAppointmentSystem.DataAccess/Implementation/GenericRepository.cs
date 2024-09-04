
using DoctorAppointmentSystem.DataAccess.Data;
using DoctorAppointmentSystem.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.DataAccess.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> dbSet;
        public GenericRepository
            (AppDbContext context)
        {
            _context = context;
            dbSet=_context.Set<T>();
        }


        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

		public async Task<int> Count()
		{
			return await dbSet.CountAsync();
		}

		public void Delete(T entity)
        {
             dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
