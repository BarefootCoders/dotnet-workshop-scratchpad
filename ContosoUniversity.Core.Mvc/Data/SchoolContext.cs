using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // No longer needed in EF Core

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors)
                .WithMany(i => i.Courses)
                .UsingEntity<Dictionary<string, object>>( // Or create a dedicated join entity class: CourseInstructor
                    "CourseInstructor", // Name of the join table
                    j => j
                        .HasOne<Instructor>()
                        .WithMany()
                        .HasForeignKey("InstructorID"),
                    j => j
                        .HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseID")
                );

            //modelBuilder.Entity<Department>().MapToStoredProcedures(); // No Longer Supported
        }
    }
}
