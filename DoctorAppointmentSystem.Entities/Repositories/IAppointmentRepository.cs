using DoctorAppointmentSystem.Entities.Models;

namespace DoctorAppointmentSystem.Entities.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    public List<Appointment> GetAppointmentsForDoctor(int DoctorId);
    public List<Appointment> GetAppointmentsForPatient(int PatientId);
}
