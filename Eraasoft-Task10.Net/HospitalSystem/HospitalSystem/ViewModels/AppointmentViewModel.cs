namespace HospitalSystem.ViewModels
{
    public class AppointmentManagementVM
    {
        public List<AppointmentDetailVM> TodayAppointments { get; set; }
        public List<AppointmentDetailVM> UpcomingAppointments { get; set; }
        public List<AppointmentDetailVM> PastAppointments { get; set; }
        public int TotalAppointmentsToday { get; set; }
        public int TotalUpcomingAppointments { get; set; }
    }

    public class AppointmentDetailVM
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string PatientGender { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}