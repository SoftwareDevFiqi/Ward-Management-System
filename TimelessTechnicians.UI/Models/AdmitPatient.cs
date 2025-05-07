using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class AdmitPatient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; } = DateTime.Now;

        [Required]
        public AdmitPatientStatus AdmitPatientStatus { get; set; } = AdmitPatientStatus.Admitted;

        [ForeignKey(nameof(Nurse))]
        public string NurseId { get; set; }

        public DateTime? DischargeDate { get; set; }
        public string? DischargeNotes { get; set; }

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; }
        public virtual ApplicationUser Nurse { get; set; } // Add this for the nurse navigation

        public virtual ICollection<PatientCondition> PatientConditions { get; set; } = new List<PatientCondition>();
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; } = new List<PatientAllergy>();
        public virtual ICollection<PatientMedication> PatientMedications { get; set; } = new List<PatientMedication>();
        public ICollection<ReAdmissionHistory> ReAdmissionHistories { get; set; }


    }

    public enum AdmitPatientStatus
    {
        Admitted,
        Discharged,
        Deleted
    }
}
