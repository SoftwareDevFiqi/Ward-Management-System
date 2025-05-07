

using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
    {
    public class AdmittedPatientViewModel
    {
        [Key]
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public DateTime AdmissionDate { get; set; }
        public AdmitPatientStatus Status { get; set; }  // Add this property

        public List<string> PatientIds { get; set; } = new List<string>();
        public List<string> Allergies { get; set; } = new List<string>();
        public List<string> Medications { get; set; } = new List<string>();
        public List<string> Conditions { get; set; } = new List<string>();
        public string NurseName { get;  set; }
        public bool IsFolderDeleted { get; set; }
    }

}


