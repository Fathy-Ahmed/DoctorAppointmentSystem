using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Entities.ViewModels;
using DoctorAppointmentSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorAppointmentSystem.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class DoctorController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IWebHostEnvironment webHostEnvironment;

	public DoctorController
		(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
	{
		this._unitOfWork = unitOfWork;
		this.userManager = userManager;
		this.webHostEnvironment = webHostEnvironment;
	}

	//////////////////////////////////////////////////////////////////////////////////

	public async Task<IActionResult> Index()
	{

		string Id = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
		var doctor = await _unitOfWork.Doctor.GetByUserId(Id);
		if (doctor.Status == SD.DoctorIsPending)
		{
			return View("DoctorPending");
		}
		return View();
	}

	//////////////////////////////////////////////////////////////////////////////////
	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Register(DoctorRegisterVM doctorVM, IFormFile file)
	{
		if (ModelState.IsValid)
		{
			string RootPath = webHostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var Upload = Path.Combine(RootPath, @"images\Doctors");
				var extension = Path.GetExtension(file.FileName);

				using (var fileStream = new FileStream(Path.Combine(Upload, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}

				doctorVM.Img = @"images\Doctors\" + fileName + extension;
			}
			Doctor doctor = new()
			{
				ClinicLocation = doctorVM.ClinicLocation,
				FirstName = doctorVM.FirstName,
				LastName = doctorVM.LastName,
				FromDay = doctorVM.FromDay,
				ToDay = doctorVM.ToDay,
				Gender = doctorVM.Gender,
				EndTime = doctorVM.EndTime,
				Img = doctorVM.Img,
				StartTime = doctorVM.StartTime,
				Qualification = doctorVM.Qualification,
				Specialties = doctorVM.Specialties,
				NumberOfPatientInDay = doctorVM.NumberOfPatientInDay,
				ExperienceYears = doctorVM.ExperienceYears,
			};
			doctor.ApplicationUserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
			_unitOfWork.Doctor.AddAsync(doctor);
			_unitOfWork.CommitChanges();
			return View("DoctorPending");
		}
		return View(doctorVM);
	}

	////////////////////////////////////////////////////////////////////////////////////
	[AllowAnonymous]
	public async Task<IActionResult> GetAll()
	{
		return View((await _unitOfWork.Doctor.GetAll()).Where(e=>e.Status==SD.DoctorIsAprovied));
	}

	////////////////////////////////////////////////////////////////////////////////////
	[AllowAnonymous]
	public async Task<IActionResult> GetById(int id)
	{
		var doctor = await _unitOfWork.Doctor.GetById(id);
		doctor.ApplicationUser = await userManager.FindByIdAsync(doctor.ApplicationUserId);
		return View(doctor);
	}
	////////////////////////////////////////////////////////////////////////////////////
	[HttpGet]
	[Authorize(Roles = SD.DoctorRole)]
	public async Task<IActionResult> EditeProfile()
	{
		string UserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
		Doctor doctor = await _unitOfWork.Doctor.GetByUserId(UserId);
		var user = await userManager.FindByIdAsync(UserId);

		DoctorVM doctorVM = new DoctorVM
		{
			FirstName=doctor.FirstName,
			LastName=doctor.LastName,
			ClinicLocation=doctor.ClinicLocation,
			EndTime=doctor.EndTime,
			ExperienceYears=doctor.ExperienceYears,
			FromDay=doctor.FromDay,
			ToDay=doctor.ToDay,
			Gender=doctor.Gender,
			Img=doctor.Img,
			NumberOfPatientInDay=doctor.NumberOfPatientInDay,
			Qualification=doctor.Qualification,
			Specialties=doctor.Specialties,
			StartTime=doctor.StartTime,
			ApplicationUserId=doctor.ApplicationUserId,

			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			UserName = user.UserName,
		};

		return View(doctorVM);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditeProfile(DoctorVM doctorVM, IFormFile? file)
	{
		if (ModelState.IsValid)
		{

			if (file != null && file.Length > 0)
			{
				string RootPath = webHostEnvironment.WebRootPath;
				string filename = Guid.NewGuid().ToString();
				var Upload = Path.Combine(RootPath, @"images\Doctors");
				var extension = Path.GetExtension(file.FileName);
				if (doctorVM.Img != null)
				{
					var OldImg = Path.Combine(RootPath, doctorVM.Img.TrimStart('\\'));
					if (System.IO.File.Exists(OldImg))
					{
						System.IO.File.Delete(OldImg);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(Upload, filename + extension), FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}
				doctorVM.Img = @"images\Doctors\" + filename + extension;
			}

			var user = await userManager.FindByIdAsync(doctorVM.ApplicationUserId);
			IdentityResult result = await userManager.SetPhoneNumberAsync(user, doctorVM.PhoneNumber);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("PhoneNumber", error.Description);
				}
			}
			result = await userManager.SetUserNameAsync(user, doctorVM.UserName);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("PhoneNumber", error.Description);
				}
			}

			Doctor doctor=await _unitOfWork.Doctor.GetByUserId(doctorVM.ApplicationUserId);
			doctor.FirstName= doctorVM.FirstName;
			doctor.LastName= doctorVM.LastName;
			doctor.Qualification= doctorVM.Qualification;
			doctor.ClinicLocation= doctorVM.ClinicLocation;
			doctor.StartTime= doctorVM.StartTime;
			doctor.EndTime= doctorVM.EndTime;
			doctor.ToDay= doctorVM.ToDay;
			doctor.FromDay= doctorVM.FromDay;
			doctor.ExperienceYears= doctorVM.ExperienceYears;
			doctor.Gender= doctorVM.Gender;
			doctor.Specialties= doctorVM.Specialties;
			doctor.Img= doctorVM.Img;
			doctor.NumberOfPatientInDay= doctorVM.NumberOfPatientInDay;

			await _unitOfWork.CommitChanges();
		}
		return View(doctorVM);
	}


	////////////////////////////////////////////////////////////////////////////////////
	public async Task<IActionResult> GetAppointments()
	{
		string UserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
		Doctor doctor = await _unitOfWork.Doctor.GetByUserId(UserId);
		List<Appointment> appointment = _unitOfWork.Appointment.GetAppointmentsForDoctor(doctor.Id);
		List<DoctorAppointmentsVM> appointmentsVM = new List<DoctorAppointmentsVM>();
		DoctorAppointmentsVM vM; 
		Patient patient;
		ApplicationUser PatientUser;

		foreach (var item in appointment)
		{
			vM = new DoctorAppointmentsVM();
			patient=new Patient();
			PatientUser=new ApplicationUser();
			patient = await _unitOfWork.Patient.GetById(item.PatientId);
			 PatientUser = await userManager.Users.FirstOrDefaultAsync(e => e.Id == patient.ApplicationUserId);

			vM.Date = item.Date;
			vM.Time = item.Time;
			vM.Status = item.Status;
			vM.Message = item.Message;
			vM.Reason = item.Reason;
			vM.PatientEmail = PatientUser.Email;
			vM.PatientName = patient.Name;
			vM.PatientPhoneNumber = PatientUser.PhoneNumber;
			vM.AppointmentId=item.Id;

			appointmentsVM.Add(vM);
		}
		return View(appointmentsVM);
	}
	////////////////////////////////////////////////////////////////////////////////////


}
