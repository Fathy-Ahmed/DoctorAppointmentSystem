using DoctorAppointmentSystem.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentSystem.Entities.Models;

public class Appointment
{
    public int Id { get; set; }


    public string? PatientName { get; set; }

    [DisplayName("What's the reason for your visit")]
    public string? Reason { get; set; }
    public TimeOnly Time { get; set; }

    [DisplayName("Appointment Date")]
    public DateOnly Date { get; set; }

    [DisplayName("Who is this appointment for")]
    public bool ForWho { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = SD.AppointmentIsNotVerified;


    //////////////////////////////////////////////////////////////////////////////////////////
    public int DoctorId { get; set; }
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; }
    //////////////////////////////////////////////////////////////////////////////////////////
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; }
    //////////////////////////////////////////////////////////////////////////////////////////

}
