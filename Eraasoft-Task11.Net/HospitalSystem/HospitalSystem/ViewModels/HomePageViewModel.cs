namespace HospitalSystem.ViewModels
{
    public class HomePageVM
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public List<string> HospitalFeatures { get; set; }
        public List<DoctorInfoVM> FeaturedDoctors { get; set; }
        public string WelcomeMessage { get; set; }
        public List<string> Services { get; set; }
    }
}