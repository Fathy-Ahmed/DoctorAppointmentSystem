

using DoctorAppointmentSystem.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.DataAccess.DbInitializer;

public interface IDbInitializer
{
    void Initialize();
}

public class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _context;

    public DbInitializer(AppDbContext context)
    {
        this._context = context;
    }
    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Count() > 0)
            {
                _context.Database.Migrate();
            }
        }
        catch (Exception)
        {
            throw;
        }

        return;
    }
}