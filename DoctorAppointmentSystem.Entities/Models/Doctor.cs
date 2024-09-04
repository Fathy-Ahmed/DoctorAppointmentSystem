using DoctorAppointmentSystem.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentSystem.Entities.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [DisplayName("Image")]
        public string Img { get; set; }
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
        public string Status { get; set; } = SD.DoctorIsPending;


        //////////////////////////////////////////////////////////////////////////////////
        public string ApplicationUserId { get;set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get;set; }



        //////////////////////////////////////////////////////////////////////////////////

        public ICollection<Appointment> Appointments { get; set; }

        //////////////////////////////////////////////////////////////////////////////////
        ///
    }
}
