using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Entities.ViewModels;
using DoctorAppointmentSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Web.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles =SD.AdminRole)]
public class AdminController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserManager<ApplicationUser> _userManager;

	public AdminController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
    {
		this._unitOfWork = unitOfWork;
		this._userManager = userManager;
	}
    //---------------------------------------------------------------------------------------------------------------------
    public async Task<IActionResult> Index()
    {
		AdminDashboardVM adminDashboardVM = new AdminDashboardVM()
		{
			UsersNum = await _unitOfWork.Patient.Count(),
			AppointmentNum=await _unitOfWork.Appointment.Count(),
			AcceptedDoctorsNum = await _unitOfWork.Doctor.CountAcceptedDoctoes(),
			RequestedDoctorsNum=await _unitOfWork.Doctor.CountRequestedDoctoes(),
		};

		return View(adminDashboardVM);
    }
	//---------------------------------------------------------------------------------------------------------------------
	public async Task<IActionResult> GetUsers()
    {
        List<Patient> patients = await _unitOfWork.Patient.GetAll();
        List<AdminUsersVM> usersVM = new List<AdminUsersVM>();
		
		AdminUsersVM userVM;

		foreach (var patient in patients)
        {
			userVM = new AdminUsersVM();
			var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == patient.ApplicationUserId);
			if (user != null)
			{
				userVM.Name = patient.Name;
				userVM.Age = patient.Age;
				userVM.Gender = patient.Gender;
				userVM.Email = user.Email;
				userVM.PhoneNumber = user.PhoneNumber;
				userVM.UserName = user.UserName;
				usersVM.Add(userVM);
			}

		}

        return View(usersVM); 
    }
	//---------------------------------------------------------------------------------------------------------------------
	public async Task<IActionResult> GetDoctors()
	{
		List<AdminDoctorsVM> adminDoctorsVM = new();
		List<Doctor> doctors= await _unitOfWork.Doctor.GetAll();
		AdminDoctorsVM DoctorVM;
		foreach (var doctor in doctors)
		{
			DoctorVM = new();
			var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == doctor.ApplicationUserId);
			if (user != null)
			{
				DoctorVM.Name=doctor.FirstName+" "+doctor.LastName;
				DoctorVM.Gender=doctor.Gender;
				DoctorVM.Specialties=doctor.Specialties;
				DoctorVM.Status = doctor.Status;

				DoctorVM.UserName = user.UserName;
				DoctorVM.Email = user.Email;
				DoctorVM.PhoneNumber = user.PhoneNumber;


				adminDoctorsVM.Add(DoctorVM);
			}

		}

		return View(adminDoctorsVM);
	}
	//---------------------------------------------------------------------------------------------------------------------
	public async Task<IActionResult> RequestsDoctors()
	{
		List<AdminRequestsDoctorVM> adminRequestsDoctorVM = new();
		List<Doctor> doctor = (await _unitOfWork.Doctor.GetAll()).Where(e => e.Status == SD.DoctorIsPending).ToList();
		AdminRequestsDoctorVM doctorNM;
		foreach (var item in doctor)
		{
			doctorNM = new()
			{
				Id = item.Id,
				Name = item.FirstName+" "+item.LastName,
				ExperienceYears = item.ExperienceYears,
				Img = item.Img,
				Qualification = item.Qualification,
				Specialties=item.Specialties
			};
			adminRequestsDoctorVM.Add(doctorNM);
		}
		return View(adminRequestsDoctorVM); 
	}
	//---------------------------------------------------------------------------------------------------------------------
	public async Task<IActionResult> ApproveDoctor(int id)
	{
		Doctor doctor= await _unitOfWork.Doctor.GetById(id);
		doctor.Status = SD.DoctorIsAprovied;
		await _unitOfWork.CommitChanges();
		return RedirectToAction("RequestsDoctors");
	}
	//---------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------

}
