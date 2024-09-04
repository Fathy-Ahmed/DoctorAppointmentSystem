using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Entities.ViewModels
{
    public class LoginVM
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RemmberMe { get; set; }
    }
}
