using static TimelessTechnicians.UI.Models.ApplicationUser;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.ViewModel
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        public Titles Title { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name cannot be longer than 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name cannot be longer than 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [PastDate(ErrorMessage = "Date of Birth cannot be in the future.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public Cities City { get; set; }

        [Required(ErrorMessage = "Suburb is required")]
        [Display(Name = "Suburb")]
        public Suburbs Suburb { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}
