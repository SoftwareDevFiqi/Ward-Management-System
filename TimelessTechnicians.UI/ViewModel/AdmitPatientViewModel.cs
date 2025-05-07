using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class AdmitPatientViewModel
    {
        [Key]
        public int Id { get; set; } // To hold the AdmitPatient Id

        [Required]
        public string PatientId { get; set; }

        public string FullName { get; set; } // To display the patient name
        public string? DischargeNotes { get; set; }
        public string SelectedNurseId { get; set; } // To hold the selected nurse
        public IEnumerable<SelectListItem> NurseList { get; set; } // For dropdown of nurses

        [Required]
        public DateTime AdmissionDate { get; set; }

        [Required]
        public AdmitPatientStatus Status { get; set; } // Holds the status of admission
        public List<int> SelectedAllergies { get; set; } = new List<int>();
        public List<int> SelectedMedications { get; set; } = new List<int>();
        public List<int> SelectedConditions { get; set; } = new List<int>();

        // Update dropdown lists to be enumerable
        public IEnumerable<SelectListItem> AllergyList { get; set; }
        public IEnumerable<SelectListItem> MedicationList { get; set; }
        public IEnumerable<SelectListItem> ConditionList { get; set; }


    }

}

