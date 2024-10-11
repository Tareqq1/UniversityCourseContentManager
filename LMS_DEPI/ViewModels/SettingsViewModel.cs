using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.Models
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string NewUsername { get; set; }

        [Required(ErrorMessage = "Current Password is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string NewEmail { get; set; }
    }
}
