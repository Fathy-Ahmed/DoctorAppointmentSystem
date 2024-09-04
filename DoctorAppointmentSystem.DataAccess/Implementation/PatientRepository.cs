using DoctorAppointmentSystem.DataAccess.Data;
using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.DataAccess.Implementation;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context) : base(context)
    {
        this._context = context;
    }

    public async Task<Patient> GetByUserId(string Id)
    {
        return await _context.Patients.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
    }



}
