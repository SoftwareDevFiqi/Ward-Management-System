using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class EditPatientFolderViewModel
    {
        public string PatientId { get; set; } // ID of the patient
        public string FirstName { get; set; } // First name of the patient
        public string LastName { get; set; } // Last name of the patient
                                             // Computed property for full name
        public string FullName => $"{FirstName} {LastName}";

        public DateTime CreatedDate { get; set; } // Date when the folder was created
        public List<int> SelectedAllergies { get; set; } // List of selected allergy IDs (as integers)
        public List<int> SelectedMedications { get; set; } // List of selected medication IDs (as integers)
        public List<int> SelectedConditions { get; set; } // List of selected condition IDs (as integers)

        public List<Allergy> Allergies { get; set; } // Assuming Allergy is a class representing an allergy
        public List<Medication> Medications { get; set; } // Assuming Medication is a class representing a medication
        public List<Condition> Conditions { get; set; } // Assuming Condition is a class representing a condition
    }
}
