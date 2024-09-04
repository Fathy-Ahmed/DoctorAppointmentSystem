using DoctorAppointmentSystem.Entities.Models;
using DoctorAppointmentSystem.Entities.Repositories;
using DoctorAppointmentSystem.Entities.ViewModels;
using DoctorAppointmentSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorAppointmentSystem.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentController(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }

    ///////////////////////////////////////////////////////////////////////////
    [HttpGet]
    public async Task<IActionResult> Add(int id)
    {
        Doctor doctor=await _unitOfWork.Doctor.GetById(id);
        if (doctor == null) { 
            return Content("Error! \nDoctor not exsist");
        }
        var claim=User.Claims.FirstOrDefault(e=>e.Type==ClaimTypes.NameIdentifier);
        Patient patient = await _unitOfWork.Patient.GetByUserId(claim.Value);
        AppointmentVM appointmentVM = new AppointmentVM()
        {
            DocotrId = doctor.Id,
            ClinicLocation=doctor.ClinicLocation,
            DocotrImg=doctor.Img,
            DocotrSpecialties=doctor.Specialties,
            DocotrName=doctor.FirstName+" "+doctor.LastName,
            PatientId=patient.Id,
            PatientName=patient.Name,
        };
        return View(appointmentVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AppointmentVM appointmentVM)
    {
        if (ModelState.IsValid)
        {
            //------------------------------ Check the Date ----------------------------------------------------
            
            var day = appointmentVM.Date.DayOfWeek.ToString();

            var startDay = (await _unitOfWork.Doctor.GetById(appointmentVM.DocotrId)).FromDay;
            
            var endDay = (await _unitOfWork.Doctor.GetById(appointmentVM.DocotrId)).ToDay;
             Dictionary<string, int> DaysValue = new Dictionary<string, int>();
            
            DaysValue[SD.Saturday] = (int)Days.Saturday;  //  1
            DaysValue[SD.Sunday] = (int)Days.Sunday;     //   2
            DaysValue[SD.Monday] = (int)Days.Monday;      //  3
            DaysValue[SD.Tuesday] = (int)Days.Tuesday;   //   4
            DaysValue[SD.Wednesday] = (int)Days.Wednesday; // 5
            DaysValue[SD.Thursday] = (int)Days.Thursday;   // 6
            DaysValue[SD.Friday] = (int)Days.Friday;      //  7

            if (!(DaysValue[day]>= DaysValue[startDay])||!( DaysValue[day] <= DaysValue[endDay]))
            {
                ModelState.TryAddModelError("", $"Doctor not avilable at this day docot work from {startDay} to {endDay}");
                return View(appointmentVM);
            }
            //------------------------------ Check the Time -------------------------------------------------------------
                var startTime= (await _unitOfWork.Doctor.GetById(appointmentVM.DocotrId)).StartTime;
                var endTime= (await _unitOfWork.Doctor.GetById(appointmentVM.DocotrId)).EndTime;
            var time = new TimeSpan(appointmentVM.Time.Hour, appointmentVM.Time.Minute, appointmentVM.Time.Second);
            if(TimeSpan.Compare(time, startTime)==-1|| TimeSpan.Compare(time, endTime) == 1)
            {
                ModelState.TryAddModelError("", $"Doctor not avilable at this time docot work from {startTime} to {endTime}");
                return View(appointmentVM);
            }

            //----------------------------------------------------------------------------------------------------
            Appointment appointment = new()
            {
                Message = appointmentVM.Message,
                PatientName = appointmentVM.PatientName,
                DoctorId=appointmentVM.DocotrId,
                PatientId=appointmentVM.PatientId,
                ForWho=appointmentVM.ForWho,
                Reason=appointmentVM.Reason,
                Status=appointmentVM.Status,
                Time=appointmentVM.Time,
                Date=appointmentVM.Date,                
            };
		   await _unitOfWork.Appointment.AddAsync(appointment);
            await _unitOfWork.CommitChanges();
            return RedirectToAction("GetAppointments", "Patient", new {area="User"});
        }
        return View(appointmentVM);
    }
	///////////////////////////////////////////////////////////////////////////
	public async Task<IActionResult> Cancel(int id)
    {
        var appointment=await _unitOfWork.Appointment.GetById(id);
        if (appointment.Status != SD.AppointmentIsCanceled)
        {
            appointment.Status = SD.AppointmentIsCanceled;
            await _unitOfWork.CommitChanges();
        }
        if(User.IsInRole(SD.DoctorRole))
            return RedirectToAction("GetAppointments", "Doctor", new { area="User"});
        else //if (User.IsInRole(SD.UserRole))
            return RedirectToAction("GetAppointments", "Patient", new { area="User"});
    }
	///////////////////////////////////////////////////////////////////////////
    public async Task<IActionResult> Verify(int id)
	{
		var appointment = await _unitOfWork.Appointment.GetById(id);
        if (appointment.Status != SD.AppointmentIsVerified)
        {
            appointment.Status = SD.AppointmentIsVerified;
            await _unitOfWork.CommitChanges();
        }

        if (User.IsInRole(SD.DoctorRole))
            return RedirectToAction("GetAppointments", "Doctor", new { area = "User" });
        else //if (User.IsInRole(SD.UserRole))
            return RedirectToAction("GetAppointments", "Patient", new { area = "User" });
    }
    ///////////////////////////////////////////////////////////////////////////
    [HttpGet]
    [Authorize(Roles = SD.UserRole)]
    public async Task<IActionResult> Reschedule(int id)
    {
        Appointment appointment = (await _unitOfWork.Appointment.GetById(id));

        AppointmentReschedule appointmentReschedule = new()
        {
            AppointmentId = appointment.Id,
            Date=appointment.Date,
            Time=appointment.Time,
            DocotrId=appointment.DoctorId,
            
        };

		return View(appointmentReschedule);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
	public async Task<IActionResult> Reschedule(AppointmentReschedule appointmentReschedule)
	{
        if (ModelState.IsValid)
        {
            //------------------------------ Check the Date ----------------------------------------------------

            var day = appointmentReschedule.Date.DayOfWeek.ToString();

            var startDay = (await _unitOfWork.Doctor.GetById(appointmentReschedule.DocotrId)).FromDay;

            var endDay = (await _unitOfWork.Doctor.GetById(appointmentReschedule.DocotrId)).ToDay;
            Dictionary<string, int> DaysValue = new Dictionary<string, int>();

            DaysValue[SD.Saturday] = (int)Days.Saturday;  //  1
            DaysValue[SD.Sunday] = (int)Days.Sunday;     //   2
            DaysValue[SD.Monday] = (int)Days.Monday;      //  3
            DaysValue[SD.Tuesday] = (int)Days.Tuesday;   //   4
            DaysValue[SD.Wednesday] = (int)Days.Wednesday; // 5
            DaysValue[SD.Thursday] = (int)Days.Thursday;   // 6
            DaysValue[SD.Friday] = (int)Days.Friday;      //  7

            if (!(DaysValue[day] >= DaysValue[startDay]) || !(DaysValue[day] <= DaysValue[endDay]))
            {
                ModelState.TryAddModelError("", $"Doctor not avilable at this day docot work from {startDay} to {endDay}");
                return View(appointmentReschedule);
            }
            //------------------------------ Check the Time -------------------------------------------------------------
            var startTime = (await _unitOfWork.Doctor.GetById(appointmentReschedule.DocotrId)).StartTime;
            var endTime = (await _unitOfWork.Doctor.GetById(appointmentReschedule.DocotrId)).EndTime;
            var time = new TimeSpan(appointmentReschedule.Time.Hour, appointmentReschedule.Time.Minute, appointmentReschedule.Time.Second);
            if (TimeSpan.Compare(time, startTime) == -1 || TimeSpan.Compare(time, endTime) == 1)
            {
                ModelState.TryAddModelError("", $"Doctor not avilable at this time docot work from {startTime} to {endTime}");
                return View(appointmentReschedule);
            }

            //----------------------------------------------------------------------------------------------------

            Appointment appointment= await _unitOfWork.Appointment.GetById(appointmentReschedule.AppointmentId);

            appointment.Date= appointmentReschedule.Date;
            appointment.Time= appointmentReschedule.Time;
            await _unitOfWork.CommitChanges();

            return RedirectToAction("GetAppointments", "Patient");
        }
		return View(appointmentReschedule);
	}
	///////////////////////////////////////////////////////////////////////////

}
