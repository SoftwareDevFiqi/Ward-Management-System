using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        // Foreign key for the Patient
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual ApplicationUser Patient { get; set; } // Navigation property

        // Foreign key for the Doctor
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } // Navigation property

        public string Medication { get; set; }
        public string Dosage { get; set; }
        public DateTime DateWritten { get; set; }
        public InstructionStatus Status { get; set; }
    }




}
