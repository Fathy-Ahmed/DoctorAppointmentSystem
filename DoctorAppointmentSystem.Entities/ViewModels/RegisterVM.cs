

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Entities.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [Length(1,20)]
        [DisplayName("UserName")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmedPassword { get; set; }

        [DisplayName("Are you a Doctor")]
        public bool IsDoctor { get; set; }
    }
}
