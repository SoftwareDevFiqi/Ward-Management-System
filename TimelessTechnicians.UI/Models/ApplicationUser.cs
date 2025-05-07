using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TimelessTechnicians.UI.Services;

namespace TimelessTechnicians.UI.Models
{
    public class ApplicationUser : IdentityUser
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

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
       ErrorMessage = "The password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public UserRole Role { get; set; }

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

        [Display(Name = "User Status")]
        public UserStatus Status { get; set; }

        [Display(Name = "I agree to the Terms of Service")]
        public bool TermsOfServiceAccepted { get; set; }



        public enum UserStatus
        {
            [Display(Name = "Active")]
            Active,

            [Display(Name = "Inactive")]
            Inactive,

            [Display(Name = "Suspended")]
            Suspended,

        }
        public enum UserRole
        {
            [Display(Name = "Administrator")]
            ADMINISTRATOR,

            [Display(Name = "Ward Administrator")]
            WARDADMIN,

            [Display(Name = "Patient")]
            PATIENT,

            [Display(Name = "Nurse")]
            NURSE,

            [Display(Name = "Nursing Sister")]
            NURSINGSISTER,

            [Display(Name = "Doctor")]
            DOCTOR,

            [Display(Name = "Script Manager")]
            SCRIPTMANAGER,

            [Display(Name = "Consumables Manager")]
            CONSUMABLESMANAGER,
        }



        public enum Titles
        {
            [Display(Name = "Mr")]
            Mr,

            [Display(Name = "Mrs")]
            Mrs,

            [Display(Name = "Advocate")]
            Advocate,

            [Display(Name = "Doctor")]
            Doctor,
        }

        // Placeholder for Cities enum (you can define this based on your specific needs)
        public enum Cities
        {
            [Display(Name = "Port Elizabeth")]
            PortElizabeth,
        }

        public enum Suburbs
        {
          
           

            [Display(Name = "Marais Township")]
            MaraisTownship,

            [Display(Name = "Marchantdale")]
            Marchantdale,

            [Display(Name = "Mount Pleasant")]
            MountPleasant,

            [Display(Name = "Neave Industrial Township")]
            NeaveIndustrialTownship,

            [Display(Name = "New Brighton")]
            NewBrighton,

            [Display(Name = "Newton Park")]
            NewtonPark,

            [Display(Name = "North End")]
            NorthEnd,

            [Display(Name = "Overbaakens")]
            Overbaakens,

            [Display(Name = "Pari Park")]
            PariPark,

            [Display(Name = "Parkside")]
            Parkside,

            [Display(Name = "Parsons Hill")]
            ParsonsHill,

            [Display(Name = "Parsons Vlei")]
            ParsonsVlei,

            [Display(Name = "Perridgevale")]
            Perridgevale,

            [Display(Name = "Prince Nikiwe Township")]
            PrinceNikiweTownship,

            [Display(Name = "Redhouse")]
            Redhouse,

            [Display(Name = "Red Location")]
            RedLocation,

            [Display(Name = "Retiefville")]
            Retiefville,

            [Display(Name = "Rowallan Park")]
            RowallanPark,

            [Display(Name = "Rufane Vale")]
            RufaneVale,

            [Display(Name = "Ruperts Hope")]
            RupertsHope,

            [Display(Name = "Salisbury Park")]
            SalisburyPark,

            [Display(Name = "Salsonerville")]
            Salsonerville,

            [Display(Name = "Salt Lake")]
            SaltLake,

            [Display(Name = "Sanctor")]
            Sanctor,

            [Display(Name = "Santa")]
            Santa,

            [Display(Name = "Schauder Township")]
            SchauderTownship,

            [Display(Name = "Schoenmakers Kop")]
            SchoenmakersKop,

            [Display(Name = "Scotstoun")]
            Scotstoun,

            [Display(Name = "Sidwell")]
            Sidwell,

            [Display(Name = "South End")]
            SouthEnd,

            [Display(Name = "Southdene")]
            Southdene,

            [Display(Name = "Springdale")]
            Springdale,

            [Display(Name = "Springfield")]
            Springfield,

            [Display(Name = "St. Georges Strand")]
            StGeorgesStrand,

            [Display(Name = "Steytler Township")]
            SteytlerTownship,

            [Display(Name = "Summerstrand")]
            Summerstrand,

            [Display(Name = "Sundridge Park")]
            SundridgePark,

            [Display(Name = "Sydenham")]
            Sydenham,

            [Display(Name = "Taybank")]
            Taybank,

            [Display(Name = "Theescombe")]
            Theescombe,

            [Display(Name = "Thornhill")]
            Thornhill,

            [Display(Name = "Uitenhage Road")]
            UitenhageRoad,

            [Display(Name = "Vander Stel")]
            VanderStel,

            [Display(Name = "Walmer")]
            Walmer,

            [Display(Name = "Walmer Downs")]
            WalmerDowns,

            [Display(Name = "Walmer Heights")]
            WalmerHeights,

            [Display(Name = "Walmer Location")]
            WalmerLocation,

            [Display(Name = "Walmer Township")]
            WalmerTownship,

            [Display(Name = "Waterkloof")]
            Waterkloof,

        }
    }
}

