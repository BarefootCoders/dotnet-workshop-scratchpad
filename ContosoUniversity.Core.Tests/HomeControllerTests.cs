using ContosoUniversity.Core.MVC.Controllers;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ContosoUniversity.Tests
{
    public class HomeControllerTests
    {
        private readonly SchoolContext _context;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new SchoolContext(options);
            _controller = new HomeController(_context);
        }

        [Fact]
        public void Index_ReturnsAViewResult()
        {
            var result = _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task About_ReturnsAViewResult_WithEnrollmentDateGroupData()
        {
            // Arrange
            _context.Students.Add(new Student
            {
                FirstMidName = "Carson",
                LastName = "Alexander",
                EnrollmentDate = DateTime.Parse("2024-01-15") 
            });
            _context.Students.Add(new Student
            {
                FirstMidName = "Meredith",
                LastName = "Alonso",
                EnrollmentDate = DateTime.Parse("2024-01-15")
            });
            _context.Students.Add(new Student
            {
                FirstMidName = "Arturo",
                LastName = "Anand",
                EnrollmentDate = DateTime.Parse("2023-09-01")
            });
            await _context.SaveChangesAsync();


            // Act
            var result = await _controller.About();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<EnrollmentDateGroup>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }



        [Fact]
        public void Contact_ReturnsAViewResult_WithViewDataMessage()
        {

            var result = _controller.Contact();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Your contact page.", viewResult.ViewData["Message"]);
        }

        [Fact]
        public void Error_ReturnsAViewResult()
        {
            var result = _controller.Error();

            Assert.IsType<ViewResult>(result);
        }
    }
}
