using DoctorAppointmentSystem.DataAccess.Data;
using DoctorAppointmentSystem.DataAccess.DbInitializer;
using DoctorAppointmentSystem.DataAccess.Implementation;
using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


            builder.Services.AddDbContext<AppDbContext>(op => {
                op.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
                });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(op =>
            {
                op.Password.RequireUppercase=false;
                op.Password.RequireNonAlphanumeric=false;
                op.Password.RequireLowercase=false;
                op.Password.RequireLowercase=false;
                op.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            SeedDb();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");


            app.Run();


            void SeedDb()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var DbInsitializer=scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    DbInsitializer.Initialize();
                }
            }



        }
        
    }
}
