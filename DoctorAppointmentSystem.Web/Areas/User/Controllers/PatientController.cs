using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Entities.ViewModels;
using DoctorAppointmentSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorAppointmentSystem.Web.Areas.User.Controllers;

[Area("User")]
public class PatientController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> userManager;

    public PatientController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
    {
        this._unitOfWork = unitOfWork;
        this.userManager = userManager;
    }

    //////////////////////////////////////////////////////////////////////////////////
    public IActionResult Index()
    {
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
    public async Task<IActionResult> Register(PatientLoginVM patientVM)
    {
        
        if (ModelState.IsValid)
        {
            Patient patient = new()
            {
                Age = patientVM.Age,
                Gender = patientVM.Gender,
                MedicalHistory = patientVM.MedicalHistory,
                Name = patientVM.Name,
                ApplicationUserId= User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value
            };
            await _unitOfWork.Patient.AddAsync(patient);
            await _unitOfWork.CommitChanges();
            return RedirectToAction("Index", "Home", new {area= "User" });
        }
        return View();
    }

    //////////////////////////////////////////////////////////////////////////////////
    
    [HttpGet]
    [Authorize(Roles = SD.UserRole)]
    public async Task<IActionResult> EditeProfile()
    {
        string UserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;

        Patient patient = await _unitOfWork.Patient.GetByUserId(UserId);

        var user = await userManager.FindByIdAsync(UserId);

        PatientVM patientVM = new PatientVM
        {
            Patient=patient,
            Email=user.Email,
            UserName=user.UserName,
            PhoneNumber = user.PhoneNumber
        };

        return View(patientVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
	public async Task<IActionResult> EditeProfile(PatientVM patientVM)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByIdAsync(patientVM.Patient.ApplicationUserId);

            IdentityResult result = await userManager.SetPhoneNumberAsync(user, patientVM.PhoneNumber);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("PhoneNumber", error.Description);
                }
            }
            result = await userManager.SetUserNameAsync(user, patientVM.UserName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("PhoneNumber", error.Description);
                }
            }

            _unitOfWork.Patient.Update(patientVM.Patient);
            await _unitOfWork.CommitChanges();
        }
        return View(patientVM);
    }
    //////////////////////////////////////////////////////////////////////////////////
    [Authorize(Roles =SD.UserRole)]
    public async Task<IActionResult> GetAppointments()
    {
		string UserId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value;
        Doctor doctor;
        Patient patient = await _unitOfWork.Patient.GetByUserId(UserId);
        IEnumerable <Appointment> appointments=_unitOfWork.Appointment.GetAppointmentsForPatient(patient.Id).Where(e=>e.Status!=SD.AppointmentIsCanceled).OrderByDescending(e=>e.Date).ThenByDescending(e=>e.Time);
        PatientAppointmentsVM appointmentsVM;
        List<PatientAppointmentsVM> appointmentList=new List<PatientAppointmentsVM>();
        foreach (var appointment in appointments)
        {
            appointmentsVM = new PatientAppointmentsVM();
            doctor = await _unitOfWork.Doctor.GetById(appointment.DoctorId);
            var UserDoctor= await userManager.Users.FirstOrDefaultAsync(e=>e.Id==doctor.ApplicationUserId);

            appointmentsVM.AppointmentId = appointment.Id;
            appointmentsVM.Time = appointment.Time;
            appointmentsVM.Date = appointment.Date;
            appointmentsVM.Status = appointment.Status;
            appointmentsVM.Reason = appointment.Reason;
            //-----------------------------------------
            appointmentsVM.DoctorPhoneNumber = UserDoctor.PhoneNumber;
            appointmentsVM.ClinicLocation = doctor.ClinicLocation;
            appointmentsVM.DoctorClinicName = "I Will Add it";
            appointmentsVM.DoctorImg = doctor.Img;
            appointmentsVM.DoctorName = doctor.FirstName+" "+doctor.LastName;
            appointmentsVM.DoctorId = doctor.Id;
            appointmentList.Add(appointmentsVM);
        }
        return View(appointmentList); 
    }
    //////////////////////////////////////////////////////////////////////////////////
}
