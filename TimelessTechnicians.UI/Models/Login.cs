using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class Login
    {
        [Display(Name = "Username or Email Address")]
        [Required(ErrorMessage = "Username or email is required.")]
        public string UserNameorEmail { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }
    }
}
