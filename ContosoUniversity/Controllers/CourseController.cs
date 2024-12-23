using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Controller for managing courses.
    /// </summary>
    public class CourseController : Controller
    {
        private SchoolContext db = new SchoolContext();

        /// <summary>
        /// Displays a list of courses, optionally filtered by department.
        /// </summary>
        /// <param name="SelectedDepartment">The ID of the department to filter by.</param>
        /// <returns>A view containing the list of courses.</returns>
        // GET: Course
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Course> courses = db.Courses
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);
            var sql = courses.ToString();
            return View(courses.ToList());
        }

        /// <summary>
        /// Displays the details of a specific course.
        /// </summary>
        /// <param name="id">The ID of the course.</param>
        /// <returns>A view containing the course details, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the course is not found.</returns>
        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Displays a form for creating a new course.
        /// </summary>
        /// <returns>A view containing the course creation form.</returns>
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="course">The course data to create.</param>
        /// <returns>Redirects to the Index action if successful, or returns the create view with error messages if the model state is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")]Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        /// <summary>
        /// Displays a form for editing an existing course.
        /// </summary>
        /// <param name="id">The ID of the course to edit.</param>
        /// <returns>A view containing the course edit form, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the course is not found.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        /// <summary>
        /// Edits an existing course.
        /// </summary>
        /// <param name="id">The ID of the course to edit.</param>
        /// <returns>Redirects to the Index action if successful, or returns the edit view with error messages if the model state is invalid or an exception occurs.</returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var courseToUpdate = db.Courses.Find(id);
            if (TryUpdateModel(courseToUpdate, "",
               new string[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        /// <summary>
        /// Populates the Department dropdown list for the Create and Edit views.
        /// </summary>
        /// <param name="selectedDepartment">The currently selected department ID.</param>
        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        /// <summary>
        /// Displays a confirmation page for deleting a course.
        /// </summary>
        /// <param name="id">The ID of the course to delete.</param>
        /// <returns>A view containing the course deletion confirmation page, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the course is not found.</returns>
        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        /// <param name="id">The ID of the course to delete.</param>
        /// <returns>Redirects to the Index action.</returns>
        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays a view for updating course credits.
        /// </summary>
        /// <returns>A view for updating course credits.</returns>
        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        /// <summary>
        /// Updates course credits by a specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier to apply to course credits.</param>
        /// <returns>The same view, displaying the number of rows affected.</returns>
        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewBag.RowsAffected = db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
