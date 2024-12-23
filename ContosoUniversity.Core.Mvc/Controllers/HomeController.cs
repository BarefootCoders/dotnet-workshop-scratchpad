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
            FormattableString query = $"SELECT EnrollmentDate, COUNT(*) AS StudentCount FROM Person WHERE Discriminator = 'Student' GROUP BY EnrollmentDate";

            IEnumerable<EnrollmentDateGroup> data = _db.Database.SqlQuery<EnrollmentDateGroup>(query);

            return View(data.ToList());
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
