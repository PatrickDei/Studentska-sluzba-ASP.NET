using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentskaSluzba.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentskaSluzba.DAL
{
    public class StudentManagerDbContext : IdentityDbContext<AppUser>
    {
        public StudentManagerDbContext(DbContextOptions<StudentManagerDbContext> options)
           : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Class> Classes { get; set; }

        public DbSet<Professor> Professors { get; set; }

        //public DbSet<Client> Clients { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>().HasData(new Course { ID = 1, Name = "Informatika" });
            modelBuilder.Entity<Course>().HasData(new Course { ID = 2, Name = "Računalstvo" });
            modelBuilder.Entity<Course>().HasData(new Course { ID = 3, Name = "Elektrotehnika" });
            modelBuilder.Entity<Course>().HasData(new Course { ID = 4, Name = "Mehatronika" });
        }
    }
}
