using System.Diagnostics;
using HospitalSystem.Data;
using HospitalSystem.Models;
using HospitalSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalDbContext _context;

        public DoctorsController()
        {
            _context = new HospitalDbContext();
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors.Include(d => d.Appointments).ToListAsync();
            
            var doctorListVM = new DoctorListVM()
            {
                Doctors = doctors.Select(d => new DoctorInfoVM
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialization = d.Specialization,
                    Img = d.Img,
                    TotalAppointments = d.Appointments?.Count ?? 0,
                    IsAvailable = true // You can add logic to determine availability
                }).ToList(),
                TotalDoctors = doctors.Count,
                SuccessMessage = TempData["SuccessMessage"]?.ToString()
            };

            return View(doctorListVM);
        }

        public async Task<IActionResult> BookAppointment(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var bookAppointmentVM = new BookAppointmentVM()
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Name,
                DoctorSpecialization = doctor.Specialization,
                DoctorImg = doctor.Img,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = new TimeOnly(9, 0), // Default to 9 AM
                PatientName = "",
                PatientAge = 0,
                PatientGender = "",
                PatientAddress = ""
            };

            return View(bookAppointmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(BookAppointmentVM bookAppointmentVM)
        {
            if (!ModelState.IsValid)
            {
                return View(bookAppointmentVM);
            }

            // Create or find patient
            var patient = new Patient
            {
                Name = bookAppointmentVM.PatientName,
                Age = bookAppointmentVM.PatientAge,
                Gender = bookAppointmentVM.PatientGender,
                Address = bookAppointmentVM.PatientAddress
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Create appointment
            var appointment = new Appointment
            {
                Date = bookAppointmentVM.AppointmentDate,
                Time = bookAppointmentVM.AppointmentTime,
                DoctorId = bookAppointmentVM.DoctorId,
                PatientId = patient.Id
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment booked successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AppointmentManagement()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();

            var appointmentManagementVM = new AppointmentManagementVM()
            {
                TodayAppointments = appointments
                    .Where(a => a.Date == today)
                    .Select(a => new AppointmentDetailVM
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Time = a.Time,
                        PatientName = a.Patient.Name,
                        PatientAge = a.Patient.Age,
                        PatientGender = a.Patient.Gender,
                        DoctorName = a.Doctor.Name,
                        DoctorSpecialization = a.Doctor.Specialization,
                        Status = "Scheduled"
                    }).ToList(),
                UpcomingAppointments = appointments
                    .Where(a => a.Date > today)
                    .Select(a => new AppointmentDetailVM
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Time = a.Time,
                        PatientName = a.Patient.Name,
                        PatientAge = a.Patient.Age,
                        PatientGender = a.Patient.Gender,
                        DoctorName = a.Doctor.Name,
                        DoctorSpecialization = a.Doctor.Specialization,
                        Status = "Upcoming"
                    }).ToList(),
                PastAppointments = appointments
                    .Where(a => a.Date < today)
                    .Select(a => new AppointmentDetailVM
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Time = a.Time,
                        PatientName = a.Patient.Name,
                        PatientAge = a.Patient.Age,
                        PatientGender = a.Patient.Gender,
                        DoctorName = a.Doctor.Name,
                        DoctorSpecialization = a.Doctor.Specialization,
                        Status = "Completed"
                    }).ToList()
            };

            appointmentManagementVM.TotalAppointmentsToday = appointmentManagementVM.TodayAppointments.Count;
            appointmentManagementVM.TotalUpcomingAppointments = appointmentManagementVM.UpcomingAppointments.Count;

            return View(appointmentManagementVM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}