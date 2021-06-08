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
    public class StudentController : Controller
    {
        private StudentManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public StudentController(StudentManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var studentQuery = this._dbContext.Students.Include(s => s.Course).AsQueryable();

            var model = studentQuery.ToList();
            return View(model: model);
        }

        [HttpPost]
        public IActionResult Index(string search)
        {
            IQueryable<Student> studentQuery;
            if (search != null) { 
                studentQuery = this._dbContext.Students
                    .Where(s => 
                            s.FirstName.ToLower().Contains(search.ToLower())
                            || s.LastName.ToLower().Contains(search.ToLower())
                            || s.JMBAG.ToLower().Contains(search.ToLower())
                            || s.Course.Name.ToLower().Contains(search.ToLower())
                            || s.DateOfEnrollment.Year.ToString().Contains(search.ToLower()))
                        .Include(s => s.Course).AsQueryable();
            }
            else
            {
                studentQuery = this._dbContext.Students
                        .Include(s => s.Course).AsQueryable();
            }

            var model = studentQuery.ToList();
            return View(model: model);
        }

        [HttpPost]
        public IActionResult IndexAjax(string search)
        {
            var studentQuery = this._dbContext.Students.Include(p => p.Course).AsQueryable();

            search = search ?? "";

            if (!string.IsNullOrWhiteSpace(search))
                studentQuery = studentQuery.Where(s => s.FirstName.ToLower().Contains(search.ToLower())
                            || s.LastName.ToLower().Contains(search.ToLower())
                            || s.JMBAG.ToLower().Contains(search.ToLower())
                            || s.Course.Name.ToLower().Contains(search.ToLower())
                            || s.DateOfEnrollment.Year.ToString().Contains(search.ToLower()));

            var model = studentQuery.ToList();
            return PartialView("_IndexTable", model);
        }

        public IActionResult Details(int id)
        {
            var student = this._dbContext.Students.Where(s => s.ID == id).Include(s => s.Course).Include(s => s.Classes).First();

            return View(model: student);
        }

        [Authorize(Roles = "Admin, Manager, Professor")]
        public IActionResult Edit(int id)
        {
            var student = this._dbContext.Students.Where(s => s.ID == id).Include(s => s.Course).First();

            FillDropdownValues();

            return View(model: student);
        }

        [Authorize(Roles = "Admin, Manager, Professor")]

        [HttpPost]
        public async Task<IActionResult> EditClasses(Student model)
        {
            if (ModelState.IsValid)
            {
                var student = this._dbContext.Students.Include(student => student.Classes).FirstOrDefault(c => c.ID == model.ID);
                model.Classes = student.Classes;

                var ok = await this.TryUpdateModelAsync(student);

                if (ok && this.ModelState.IsValid)
                {
                    this._dbContext.SaveChanges();
                    List<int> classes = student.Classes.Select(c => c.ID).ToList();
                    ViewBag.Classes = this._dbContext.Classes.Where(c => classes.Contains(c.ID) == false).ToList();
                    return View(model: student);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.FillDropdownValues();
                return View("Edit");
            }
        }

        [Authorize(Roles = "Admin, Manager, Professor")]

        [HttpPost]
        public async Task<IActionResult> EditClassesApply(int id,  List<int> classID)
        {
            var student = this._dbContext.Students.FirstOrDefault(s => s.ID == id);

            ICollection<Class> classes = new Collection<Class>();

            foreach (var c in classID)
                classes.Add(this._dbContext.Classes.FirstOrDefault(a => a.ID == c));

            student.Classes = classes;
            
            var ok = await this.TryUpdateModelAsync(student);

            if (ok)
            {
                this._dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Manager")]

        public IActionResult Create()
        {
            FillDropdownValues();

            return View(new Student());
        }

        [Authorize(Roles = "Admin, Manager")]

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                this._dbContext.Students.Add(student);

                this._dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                FillDropdownValues();
                return View();
            }
        }

        private void FillDropdownValues()
        {
            var selectItems = new List<SelectListItem>();

            foreach (var course in this._dbContext.Courses)
            {
                var listItem = new SelectListItem(course.Name, course.ID.ToString());
                selectItems.Add(listItem);
            }

            ViewBag.PossibleCourses = selectItems;
        }
    }
}
