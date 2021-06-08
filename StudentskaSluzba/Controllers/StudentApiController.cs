using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentskaSluzba.DAL;
using StudentskaSluzba.Model;
using StudentskaSluzba.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace StudentskaSluzba.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentApiController : Controller
    {
        private StudentManagerDbContext _dbContext;

        public StudentApiController(StudentManagerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IActionResult Get()
        {
            var students = this._dbContext.Students.Include(s => s.Course).Include(s => s.Classes)
                .Select(o => new StudentDTO() { 
                    ID = o.ID,
                    FullName = o.FirstName + " " + o.LastName,
                    JMBAG = o.JMBAG,
                    DateOfBirth = o.DateOfBirth,
                    Course = new CourseDTO() { 
                        Name = o.Course.Name
                    },
                    Classes = o.Classes.Select( c => new ClassDTO() { 
                        Name = c.Name
                    }).ToList()
                })
                .ToList();

            return Ok(students);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Student student)
        {
            if (ModelState.IsValid)
            {
                this._dbContext.Students.Add(student);

                this._dbContext.SaveChanges();
            }

            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Put(int id, [FromBody] Student student)
        {
            var studentToUpdate = this._dbContext.Students.First(s => s.ID == id);

            studentToUpdate.TryUpdateStudent(student);

            this._dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var studentToRemove = this._dbContext.Students.First(s => s.ID == id);

            this._dbContext.Students.Remove(studentToRemove);

            this._dbContext.SaveChanges();

            return Ok();
        }
    }

    public class StudentDTO
    {
        public int ID { get; set; }
        public string FullName { get; set; }

        public string JMBAG { get; set; }

        public DateTime DateOfBirth { get; set; }

        public List<ClassDTO> Classes { get; set; }

        public CourseDTO Course { get; set; }
    }

    public class ClassDTO
    {
        public string Name { get; set; }
    }

    public class CourseDTO
    {
        public string Name { get; set; }
    }
}
