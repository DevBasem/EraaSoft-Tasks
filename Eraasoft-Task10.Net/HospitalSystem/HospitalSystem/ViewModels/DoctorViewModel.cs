namespace HospitalSystem.ViewModels
{
    public class DoctorListVM
    {
        public List<DoctorInfoVM> Doctors { get; set; }
        public int TotalDoctors { get; set; }
        public string SuccessMessage { get; set; }
    }

    public class DoctorInfoVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Img { get; set; }
        public int TotalAppointments { get; set; }
        public bool IsAvailable { get; set; }
    }
}