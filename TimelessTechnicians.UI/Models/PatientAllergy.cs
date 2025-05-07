using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelessTechnicians.UI.Models
{
    public class PatientAllergy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdmitPatientId { get; set; }
        [ForeignKey(nameof(AdmitPatientId))]
        public AdmitPatient AdmitPatient { get; set; }

        [Required]
        public int AllergyId { get; set; }
        [ForeignKey(nameof(AllergyId))]
        public Allergy Allergy { get; set; }

        [Required]
        public string PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; }

        public RecordStatus Status { get; set; } = RecordStatus.Active;
    }
}
