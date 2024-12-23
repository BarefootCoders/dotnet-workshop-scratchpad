using ContosoUniversity.Controllers;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsAViewResult()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new SchoolContext(options);
            var controller = new HomeController(context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void About_ReturnsAViewResult_WithEnrollmentData()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new SchoolContext(options);
            context.Students.Add(new Student { EnrollmentDate = System.DateTime.Now });
            context.SaveChanges();

            var controller = new HomeController(context);


            var result = controller.About();

            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<EnrollmentDateGroup>>((result as ViewResult).Model);
            Assert.True(model.Count > 0);

        }

        [Fact]
        public void Contact_ReturnsAViewResult_WithData()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new SchoolContext(options);

            var controller = new HomeController(context);

            var result = controller.Contact();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Your contact page.", viewResult.ViewData["Message"]);
        }
    }
}
