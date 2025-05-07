using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.ViewModel
{
    public class AssignBedViewModel
    {
        [Key]
        public int Id { get; set; }
        public int AdmitPatientId { get; set; }
        public string PatientId { get; set; } // For patient dropdown
        public int BedId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public List<SelectListItem> PatientList { get; set; } // Dropdown for patients
        public List<SelectListItem> BedList { get; set; } // Dropdown for beds
    }
}
