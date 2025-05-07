using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.ViewModel
{
    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string CurrentEmail { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        [Compare("NewEmail", ErrorMessage = "The new email and confirmation email do not match.")]
        [EmailAddress]
        public string ConfirmNewEmail { get; set; }
    }

}
