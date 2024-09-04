using DoctorAppointmentSystem.DataAccess.Data;
using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Entities.ViewModels;

namespace DoctorAppointmentSystem.DataAccess.Implementation;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) : base(context)
    {
        this._context = context;
    }

	public List<Appointment> GetAppointmentsForDoctor(int DoctorId)
	{
		return _context.Appointments.Where(e=>e.DoctorId == DoctorId).ToList();
	}

	public List<Appointment> GetAppointmentsForPatient(int PatientId)
	{
		return _context.Appointments.Where(e => e.PatientId == PatientId).ToList();
	}
}
