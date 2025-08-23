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

            // Check for duplicate appointments (same doctor, date, and time) - Enhanced doctor availability check
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

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction("AppointmentManagement");
                }

                // Check if appointment is in the past
                var today = DateOnly.FromDateTime(DateTime.Today);
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                
                if (appointment.Date < today || 
                    (appointment.Date == today && appointment.Time <= currentTime))
                {
                    TempData["ErrorMessage"] = "Cannot cancel past appointments.";
                    return RedirectToAction("AppointmentManagement");
                }

                // Remove the appointment
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Appointment for {appointment.Patient.Name} on {appointment.Date:MMMM dd, yyyy} at {appointment.Time:hh:mm tt} has been cancelled successfully. Dr. {appointment.Doctor.Name} is now available at this time.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while cancelling the appointment. Please try again.";
            }

            return RedirectToAction("AppointmentManagement");
        }

        // API endpoint to get available time slots for a specific doctor and date
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, string date)
        {
            try
            {
                if (!DateOnly.TryParse(date, out var appointmentDate))
                {
                    return BadRequest("Invalid date format");
                }

                // Get all existing appointments for this doctor on this date
                var existingAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.Date == appointmentDate)
                    .Select(a => a.Time)
                    .ToListAsync();

                // Define all possible time slots (9:00 AM to 5:00 PM in 30-minute intervals)
                var allTimeSlots = new List<TimeOnly>();
                for (int hour = 9; hour <= 17; hour++)
                {
                    if (hour < 17)
                    {
                        allTimeSlots.Add(new TimeOnly(hour, 0));
                        allTimeSlots.Add(new TimeOnly(hour, 30));
                    }
                    else
                    {
                        allTimeSlots.Add(new TimeOnly(hour, 0)); // 5:00 PM
                    }
                }

                // If the appointment is for today, remove past time slots
                if (appointmentDate == DateOnly.FromDateTime(DateTime.Today))
                {
                    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    allTimeSlots = allTimeSlots.Where(t => t > currentTime.AddMinutes(30)).ToList();
                }

                // Remove already booked time slots
                var availableTimeSlots = allTimeSlots
                    .Where(t => !existingAppointments.Contains(t))
                    .Select(t => new { 
                        value = t.ToString("HH:mm"), 
                        text = t.ToString("hh:mm tt") 
                    })
                    .ToList();

                return Json(availableTimeSlots);
            }
            catch (Exception ex)
            {
                return BadRequest("Error retrieving available time slots");
            }
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