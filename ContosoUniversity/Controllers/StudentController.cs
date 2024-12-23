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
using PagedList;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Controller for managing student data.
    /// </summary>
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        /// <summary>
        /// Displays a list of students, optionally filtered and sorted, with paging.
        /// </summary>
        /// <param name="sortOrder">The sort order for the student list.</param>
        /// <param name="currentFilter">The current filter string.</param>
        /// <param name="searchString">The search string to filter students by.</param>
        /// <param name="page">The current page number.</param>
        /// <returns>A view containing the paged list of students.</returns>
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Displays the details of a specific student.
        /// </summary>
        /// <param name="id">The ID of the student.</param>
        /// <returns>A view containing the student details, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the student is not found.</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }


        /// <summary>
        /// Displays a form for creating a new student.
        /// </summary>
        /// <returns>A view containing the student creation form.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="student">The student data to create.</param>
        /// <returns>Redirects to the Index action if successful, otherwise returns the create view with error messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        /// <summary>
        /// Displays a form for editing an existing student.
        /// </summary>
        /// <param name="id">The ID of the student to edit.</param>
        /// <returns>A view containing the student edit form, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the student is not found.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        /// <summary>
        /// Edits an existing student.
        /// </summary>
        /// <param name="id">The ID of the student to edit.</param>
        /// <returns>Redirects to the Index action if successful, otherwise returns the edit view with error messages.</returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
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
            return View(studentToUpdate);
        }

        /// <summary>
        /// Displays a confirmation page for deleting a student.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <param name="saveChangesError">Indicates whether an error occurred during a previous delete attempt.</param>
        /// <returns>A view containing the student deletion confirmation page, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the student is not found.</returns>
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        /// <summary>
        /// Deletes a student.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <returns>Redirects to the Index action if successful, otherwise redirects to the Delete action with an error indicator.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
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
