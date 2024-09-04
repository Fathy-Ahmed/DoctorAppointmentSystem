using DoctorAppointmentSystem.Utilities;
using System.ComponentModel;

namespace DoctorAppointmentSystem.Entities.ViewModels;

public class AppointmentVM
{
    public string? PatientName { get; set; }

    [DisplayName("What's the reason for your visit")]
    public string? Reason { get; set; }
    public TimeOnly Time { get; set; }

    [DisplayName("Appointment Date")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [DisplayName("Who is this appointment for")]
    public bool ForWho { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = SD.AppointmentIsNotVerified;
    ////////////////////////////////////////////////////////
    public int DocotrId { get; set; }
    public string DocotrName { get; set; }
    public string DocotrImg { get; set; }
    public string DocotrSpecialties { get; set; }
    public string ClinicLocation { get; set; }
    ////////////////////////////////////////////////////////
    public int PatientId { get; set; }
}
