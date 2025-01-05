using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging; // Added logging

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Controller for managing courses.
    /// </summary>
    public class CourseController : Controller
    {
        private readonly SchoolContext _db;
         private readonly ILogger<CourseController> _logger;

        // Constructor injection for SchoolContext and Logger
        public CourseController(SchoolContext db, ILogger<CourseController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of courses, optionally filtered by department.
        /// </summary>
        /// <param name="SelectedDepartment">The ID of the department to filter by.</param>
        /// <returns>A view containing the list of courses.</returns>
        public IActionResult Index(int? selectedDepartment)
        {
            var departments = _db.Departments.OrderBy(q => q.Name).ToList();
            ViewData["SelectedDepartment"] = new SelectList(departments, "DepartmentID", "Name", selectedDepartment);
            int departmentId = selectedDepartment.GetValueOrDefault();

            IQueryable<Course> courses = _db.Courses
                .Where(c => !selectedDepartment.HasValue || c.DepartmentID == departmentId)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);

            return View(courses.ToList());
        }

        /// <summary>
        /// Displays the details of a specific course.
        /// </summary>
        /// <param name="id">The ID of the course.</param>
        /// <returns>A view containing the course details, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the course is not found.</returns>
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult(); // Or StatusCode(400);
            }
            Course course = _db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Displays a form for creating a new course.
        /// </summary>
        /// <returns>A view containing the course creation form.</returns>
        public IActionResult Create()
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
        public IActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Courses.Add(course);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                //Log the error
                _logger.LogError(ex, "Unable to save changes while creating course.");
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                 return new BadRequestResult(); // Or StatusCode(400);
            }
            Course course = _db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
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
        public IActionResult EditPost(int? id)
        {
             if (id == null)
            {
                return new BadRequestResult();
            }
            var courseToUpdate = _db.Courses.Find(id);
             if (TryUpdateModel(courseToUpdate, "",
               new string[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                 catch (DbUpdateException ex)
                {
                    //Log the error
                      _logger.LogError(ex, "Unable to save changes while editing course.");
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
            var departmentsQuery = _db.Departments
                                   .OrderBy(d => d.Name)
                                   .ToList();

            ViewData["DepartmentID"] = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        /// <summary>
        /// Displays a confirmation page for deleting a course.
        /// </summary>
        /// <param name="id">The ID of the course to delete.</param>
        /// <returns>A view containing the course deletion confirmation page, or an HTTP 400 error if the ID is null, or an HTTP 404 error if the course is not found.</returns>
        public IActionResult Delete(int? id)
        {
           if (id == null)
            {
                return new BadRequestResult();
            }
            Course course = _db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        /// <param name="id">The ID of the course to delete.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public IActionResult DeleteConfirmed(int id)
        {
             Course course = _db.Courses.Find(id);
            _db.Courses.Remove(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


       /// <summary>
        /// Displays a view for updating course credits.
        /// </summary>
        /// <returns>A view for updating course credits.</returns>
        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        /// <summary>
        /// Updates course credits by a specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier to apply to course credits.</param>
        /// <returns>The same view, displaying the number of rows affected.</returns>
        [HttpPost]
        public IActionResult UpdateCourseCredits(int? multiplier)
        {
             if (multiplier != null)
            {
                 ViewBag.RowsAffected = _db.Database.ExecuteSqlRaw("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
             return View();
        }

        // Dispose is no longer required

    }
}
