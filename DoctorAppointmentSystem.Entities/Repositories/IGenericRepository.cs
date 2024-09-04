

namespace DoctorAppointmentSystem.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public  Task<List<T>> GetAll();
        public Task<T> GetById(int id);

        public Task AddAsync(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public Task<int> Count();
		// public Task DeleteRangeAsync(List<T> entities);

	}
}
