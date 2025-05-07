using Microsoft.AspNetCore.Mvc.Rendering;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class EditAdmissionFolderViewModel
    {
        public int AdmitPatientId { get; set; } // Unique ID for the AdmitPatient record

        public string PatientId { get; set; } // Patient identifier

        public DateTime AdmissionDate { get; set; } // Admission date of the patient

        public int SelectedAllergy { get; set; } // Selected Allergy ID
        public List<SelectListItem> AllergyList { get; set; } // Dropdown list for allergies

        public int SelectedMedication { get; set; } // Selected Medication ID
        public List<SelectListItem> MedicationList { get; set; } // Dropdown list for medications

        public int SelectedCondition { get; set; } // Selected Condition ID
        public List<SelectListItem> ConditionList { get; set; } // Dropdown list for conditions

        public AdmitPatientStatus Status { get; set; } // Current admission status (e.g., Admitted, Discharged)

        // For status dropdown if you want to allow administrators to change the status
        public List<SelectListItem> StatusList { get; set; }

        // Additional properties if needed
        public string NurseId { get; set; } // ID of the assigned nurse
        public List<SelectListItem> NurseList { get; set; } // Dropdown list for nurses
    }

}
