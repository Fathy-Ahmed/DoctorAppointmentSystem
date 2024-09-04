using DoctorAppointmentSystem.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Entities.ViewModels;

public class PatientVM
{
    public Patient Patient { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.PhoneNumber)]
    [DisplayName("Phone Number")]
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
}