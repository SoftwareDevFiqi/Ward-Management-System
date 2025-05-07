using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class PatientMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; }

        [Required]
        public int MedicationId { get; set; }
        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateAdministered { get; set; } = DateTime.Now;

        [Required]
        public int AdmitPatientId { get; set; }
        [ForeignKey(nameof(AdmitPatientId))]
        public AdmitPatient AdmitPatient { get; set; }

        public RecordStatus Status { get; set; } = RecordStatus.Active;
    }


}
