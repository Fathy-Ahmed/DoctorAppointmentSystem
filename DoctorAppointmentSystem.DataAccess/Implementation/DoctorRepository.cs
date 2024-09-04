using DoctorAppointmentSystem.DataAccess.Data;
using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.DataAccess.Implementation;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(AppDbContext context) : base(context)
    {
        this._context = context;
    }

	public async Task<int> CountAcceptedDoctoes()
	{
		return await _context.Doctors.Where(e=>e.Status==SD.DoctorIsAprovied).CountAsync();
	}

	public async Task<int> CountRequestedDoctoes()
	{
		return await _context.Doctors.Where(e => e.Status == SD.DoctorIsPending).CountAsync();
	}

	public async Task<Doctor> GetByUserId(string Id)
    {
        return  await _context.Doctors.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
    }



}
