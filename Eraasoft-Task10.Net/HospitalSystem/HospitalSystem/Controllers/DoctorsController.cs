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
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), // Default to tomorrow
                AppointmentTime = "", // Empty by default, user must select
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
            // Custom validation for weekend appointments
            var appointmentDate = bookAppointmentVM.AppointmentDate.ToDateTime(TimeOnly.MinValue);
            var dayOfWeek = appointmentDate.DayOfWeek;
            
            if (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday)
            {
                ModelState.AddModelError("AppointmentDate", "We are closed on Fridays and Saturdays. Please select another date.");
            }

            // Validate appointment date is not in the past
            if (bookAppointmentVM.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("AppointmentDate", "Appointment date cannot be in the past.");
            }

            // Validate appointment time format and range
            if (!string.IsNullOrEmpty(bookAppointmentVM.AppointmentTime))
            {
                if (!TimeOnly.TryParse(bookAppointmentVM.AppointmentTime, out var parsedTime))
                {
                    ModelState.AddModelError("AppointmentTime", "Invalid time format.");
                }
                else
                {
                    // Check if time is within business hours (9 AM to 5 PM)
                    var startTime = new TimeOnly(9, 0);
                    var endTime = new TimeOnly(17, 0);
                    
                    if (parsedTime < startTime || parsedTime > endTime)
                    {
                        ModelState.AddModelError("AppointmentTime", "Appointment time must be between 9:00 AM and 5:00 PM.");
                    }

                    // If appointment is today, check if time is not in the past
                    if (bookAppointmentVM.AppointmentDate == DateOnly.FromDateTime(DateTime.Today))
                    {
                        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                        if (parsedTime <= currentTime.AddMinutes(30)) // 30 minutes buffer
                        {
                            ModelState.AddModelError("AppointmentTime", "Please select a time at least 30 minutes from now.");
                        }
                    }
                }
            }

            // Check for duplicate appointments (same doctor, date, and time)
            if (!string.IsNullOrEmpty(bookAppointmentVM.AppointmentTime) && 
                TimeOnly.TryParse(bookAppointmentVM.AppointmentTime, out var timeToCheck))
            {
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.DoctorId == bookAppointmentVM.DoctorId && 
                                            a.Date == bookAppointmentVM.AppointmentDate && 
                                            a.Time == timeToCheck);
                
                if (existingAppointment != null)
                {
                    ModelState.AddModelError("AppointmentTime", "This time slot is already booked. Please select another time.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(bookAppointmentVM);
            }

            // Parse the time string to TimeOnly for database storage
            var appointmentTime = TimeOnly.Parse(bookAppointmentVM.AppointmentTime);

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
                Time = appointmentTime,
                DoctorId = bookAppointmentVM.DoctorId,
                PatientId = patient.Id
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Appointment booked successfully for {bookAppointmentVM.AppointmentDate:MMMM dd, yyyy} at {appointmentTime:hh:mm tt}!";
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