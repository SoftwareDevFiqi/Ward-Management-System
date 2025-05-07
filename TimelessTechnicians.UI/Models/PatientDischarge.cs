namespace TimelessTechnicians.UI.Models
{
    public class PatientDischarge
    {
        public int Id { get; set; }
        public int AdmitPatientId { get; set; }
        public string DischargeReason { get; set; }
        public DateTime DischargeDate { get; set; }
        public string AdministeredBy { get; set; }
        public InstructionStatus Status { get; set; }
        public string Notes { get; set; } // Make Notes nullable

        public virtual AdmitPatient AdmitPatient { get; set; }
    }






}
