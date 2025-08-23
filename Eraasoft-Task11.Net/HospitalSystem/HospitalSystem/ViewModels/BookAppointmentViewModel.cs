using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.ViewModels
{
    public class BookAppointmentVM
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorImg { get; set; }
        
        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        public DateOnly AppointmentDate { get; set; }
        
        [Required(ErrorMessage = "Appointment time is required")]
        public string AppointmentTime { get; set; }
        
        [Required(ErrorMessage = "Patient name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string PatientName { get; set; }
        
        [Required(ErrorMessage = "Patient age is required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int PatientAge { get; set; }
        
        [Required(ErrorMessage = "Gender is required")]
        public string PatientGender { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string PatientAddress { get; set; }
    }
}