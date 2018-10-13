using System.ComponentModel.DataAnnotations;

namespace Basic.WebAPI.ViewModels
{
    public class SignupVm
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email Address.")]        
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(255, ErrorMessage = "First name must be less than 255 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(255, ErrorMessage = "Last name must be less than 255 characters.")]
        public string LastName { get; set; }
                
        [StringLength(11, ErrorMessage = "Phone must be 10 or 11 digits long.", MinimumLength = 10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, ErrorMessage = "Password must be between 5 and 255 characters", MinimumLength = 5)]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Password and confirm password must match.")]
        public string ConfirmPassword { get; set; }
    }
}