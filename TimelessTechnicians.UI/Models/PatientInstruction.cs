using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class PatientInstruction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(AdmitPatient))]
        public int AdmitPatientId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Instruction cannot exceed 500 characters.")]
        public string Instruction { get; set; }

        [Required]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;

        [Required]
        public InstructionStatus Status { get; set; } = InstructionStatus.Active;

        // Navigation property
        public virtual AdmitPatient AdmitPatient { get; set; }

        // Make AdministeredBy a required non-nullable string
        [Required]  // Ensure this field is required
        public string AdministeredBy { get; set; }  

        public InstructionType InstructionType { get; set; }
    }



    public enum InstructionStatus
    {
        Active,
        Deleted,
        Pending,
        ForwardedToPharmacy,
        Delivered,
        Completed,
        Received
    }


    public enum InstructionType
    {
        General,
        Discharge
    }
}
