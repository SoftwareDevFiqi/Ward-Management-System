using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.ViewModel
{
    public class DischargeViewModel
    {

        public int Id { get; set; }
        [Required]
        public int AdmitPatientId { get; set; }

        public string PatientFullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DischargeDate { get; set; } = DateTime.Now;

        public string Notes { get; set; }

        public IEnumerable<SelectListItem> AdmittedPatients { get; set; }
    }
}
