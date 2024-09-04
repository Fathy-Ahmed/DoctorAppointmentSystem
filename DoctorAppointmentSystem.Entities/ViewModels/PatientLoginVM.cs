

using System.ComponentModel;

namespace DoctorAppointmentSystem.Entities.ViewModels;

public class PatientLoginVM
{
    [DisplayName("Full Name")]
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    [DisplayName("Medical History")]
    public string? MedicalHistory { get; set; }

}
