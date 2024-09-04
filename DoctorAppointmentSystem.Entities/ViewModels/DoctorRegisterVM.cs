

using System.ComponentModel;

namespace DoctorAppointmentSystem.Entities.ViewModels;

public class DoctorRegisterVM
{
    public string FirstName { get; set; }
    [DisplayName("Last Name")]
    public string LastName { get; set; }
    public string Gender { get; set; }
    [DisplayName("Image")]
    public string? Img { get; set; }
    public string Specialties { get; set; }
    public string Qualification { get; set; }
    [DisplayName("Number of experience years")]
    public int ExperienceYears { get; set; }
    [DisplayName("Clinic Location")]
    public string ClinicLocation { get; set; }
    [DisplayName("From Day")]
    public string FromDay { get; set; }
    [DisplayName("To Day")]
    public string ToDay { get; set; }
    [DisplayName("Start Time")]
    public TimeSpan StartTime { get; set; }
    [DisplayName("End Time")]
    public TimeSpan EndTime { get; set; }

    [DisplayName("Number of patients per day")]
    public int NumberOfPatientInDay { get; set; }

}
