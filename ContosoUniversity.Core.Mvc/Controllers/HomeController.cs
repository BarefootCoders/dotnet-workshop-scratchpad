using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Controller for managing the home page, about page, and contact page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly SchoolContext _db; // Use dependency injection

        // Constructor injection for SchoolContext
        public HomeController(SchoolContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns>The home page view.</returns>
        public IActionResult Index() // Updated return type
        {
            return View();
        }

        /// <summary>
        /// Displays the about page with student enrollment statistics.
        /// </summary>
        /// <returns>The about page view with enrollment data.</returns>
        public IActionResult About() // Updated return type and async
        {
            var data = _db.Students
                .GroupBy(s => s.EnrollmentDate)
                .Select(g => new EnrollmentDateGroup
                {
                    EnrollmentDate = g.Key,
                    StudentCount = g.Count()
                })
                .ToList();

            return View(data);
        }

        /// <summary>
        /// Displays the contact page.
        /// </summary>
        /// <returns>The contact page view.</returns>
        public IActionResult Contact() // Updated return type
        {
            ViewData["Message"] = "Your contact page."; // Updated ViewBag to ViewData

            return View();
        }



        // Dispose method is handled by dependency injection in ASP.NET Core
    }
}
