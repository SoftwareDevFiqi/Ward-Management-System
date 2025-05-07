using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.ViewModel
{
    public class PatientVitalViewModel
    {
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; }

        [Required]
        [Display(Name = "Blood Pressure")]
        public string BloodPressure { get; set; }

        [Required]
        [Display(Name = "Temperature")]
        public string Temperature { get; set; }

        [Required]
        [Display(Name = "Sugar Level")]
        public string SugarLevel { get; set; }

        [Required]
        public DateTime RecordedDate { get; set; } = DateTime.Now;

        public PatientVitalStatus PatientVitalStatus { get; set; } = PatientVitalStatus.Active; // Default to Active
    }


}
