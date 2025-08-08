namespace HospitalSystem.ViewModels
{
    public class HospitalInfoVM
    {
        public string Name { get; set; }
        public int EstablishedYear { get; set; }
        public string Address { get; set; }
        public string ContactPhone { get; set; }
        public List<string> Departments { get; set; }
        public int TotalBeds { get; set; }
        public int TotalStaff { get; set; }
        public bool IsAccredited { get; set; }
        public string AccreditationDetails { get; set; }
        public List<string> Certifications { get; set; }
    }
}