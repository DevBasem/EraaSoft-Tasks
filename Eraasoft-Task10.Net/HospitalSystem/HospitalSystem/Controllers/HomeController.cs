using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitalSystem.Models;
using HospitalSystem.ViewModels;
using HospitalSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HospitalDbContext _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = new HospitalDbContext();
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors.Include(d => d.Appointments).ToListAsync();
            var patients = await _context.Patients.ToListAsync();
            var appointments = await _context.Appointments.ToListAsync();

            var homePageVM = new HomePageVM()
            {
                TotalDoctors = doctors.Count,
                TotalPatients = patients.Count,
                TotalAppointments = appointments.Count,
                WelcomeMessage = "Welcome to Hospital System - Your health is our priority",
                HospitalFeatures = new List<string>
                {
                    "24/7 Emergency Services",
                    "Modern Medical Equipment",
                    "Experienced Medical Staff",
                    "Comprehensive Health Checkups",
                    "Insurance Accepted",
                    "Patient-Centered Care"
                },
                Services = new List<string>
                {
                    "General Medicine",
                    "Cardiology",
                    "Pediatrics",
                    "Orthopedics",
                    "Dermatology",
                    "Neurology"
                },
                FeaturedDoctors = doctors.Take(3).Select(d => new DoctorInfoVM
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialization = d.Specialization,
                    Img = d.Img,
                    TotalAppointments = d.Appointments?.Count ?? 0,
                    IsAvailable = true
                }).ToList()
            };

            return View(homePageVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult HospitalInfo()
        {
            string hospitalName = "City General Hospital";
            int establishedYear = 1985;
            string address = "123 Healthcare Street, Medical District";
            string contactPhone = "+1-555-HOSPITAL";

            List<string> departments = new List<string>
            {
                "Emergency Medicine",
                "Internal Medicine",
                "Surgery",
                "Pediatrics",
                "Obstetrics & Gynecology",
                "Radiology"
            };

            var hospitalInfoVM = new HospitalInfoVM()
            {
                Name = hospitalName,
                EstablishedYear = establishedYear,
                Address = address,
                ContactPhone = contactPhone,
                Departments = departments,
                TotalBeds = 250,
                TotalStaff = 150,
                IsAccredited = true
            };

            return View(hospitalInfoVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
