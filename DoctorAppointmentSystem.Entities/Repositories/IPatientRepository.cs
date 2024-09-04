using DoctorAppointmentSystem.Entities.Models;

namespace DoctorAppointmentSystem.Entities.Repositories;

public interface IPatientRepository : IGenericRepository<Patient>
{
    public Task<Patient> GetByUserId(string Id);
}
