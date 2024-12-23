using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;


namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Controller for managing the home page, about page, and contact page.
    /// </summary>
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();

        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns>The home page view.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the about page with student enrollment statistics.
        /// </summary>
        /// <returns>The about page view with enrollment data.</returns>
        public ActionResult About()
        {
            string query = "SELECT EnrollmentDate, COUNT(*) AS StudentCount "
                + "FROM Person "
                + "WHERE Discriminator = 'Student' "
                + "GROUP BY EnrollmentDate";
            IEnumerable<EnrollmentDateGroup> data = db.Database.SqlQuery<EnrollmentDateGroup>(query);

            return View(data.ToList());
        }

        /// <summary>
        /// Displays the contact page.
        /// </summary>
        /// <returns>The contact page view.</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Disposes of the database context when the controller is disposed.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
