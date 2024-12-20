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
    /// Controller for handling home page, about page, and contact page requests.
    /// </summary>
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();

        /// <summary>
        /// Returns the default view for the home page.
        /// </summary>
        /// <returns>The home page view.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieves enrollment statistics and displays them on the about page.
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
        /// Returns the view for the contact page.
        /// </summary>
        /// <returns>The contact page view.</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
