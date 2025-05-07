using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Models
{
    public class MedicationPrescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdmitPatientId { get; set; }

        [ForeignKey("AdmitPatientId")]
        public virtual AdmitPatient AdmitPatient { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatePrescribed { get; set; }

        [Required]
        public string Dosage { get; set; } // Dosage details

        public MedicationStatus Status { get; set; } // To handle deletion
        public string AdministeredBy { get; set; }

    }

}
