using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TimelessTechnicians.UI.ViewModel
{
    public class PatientMovementViewModel
    {
        public int Id { get; set; }
        public int AdmitPatientId { get; set; }
        public string PatientFullName { get; set; }
        public DateTime MovementDate { get; set; }
        public string Location { get; set; }
        public MovementType MovementType { get; set; } // Use enum from ViewModel namespace
        public string Notes { get; set; }

        // For the RecordMovement GET view
        public List<SelectListItem> PatientList { get; set; }
    }

    // Enum to represent the types of movements
    public enum MovementType
    {
        CheckIn,
        CheckOut,
        Transfer,
        Emergency
    }
}
