namespace HospitalSystem.ViewModels
{
    public class PatientInfoVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public List<AppointmentSummaryVM> Appointments { get; set; }
        public int TotalAppointments { get; set; }
    }

    public class AppointmentSummaryVM
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string Status { get; set; }
    }
}