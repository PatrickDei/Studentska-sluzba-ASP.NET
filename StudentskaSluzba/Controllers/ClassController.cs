using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentskaSluzba.DAL;
using StudentskaSluzba.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentskaSluzba.Controllers
{
    public class ClassController : Controller
    {
        private StudentManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public ClassController(StudentManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            var classQuery = this._dbContext.Classes.Include(c => c.Course).AsQueryable();

            var model = classQuery.ToList();

            return View(model: model);
        }

        [HttpPost]
        public IActionResult Index(string search)
        {
            IQueryable<Class> studentQuery;
            if (search != null)
            {
                studentQuery = this._dbContext.Classes
                    .Where(s =>
                        s.Name.ToLower().Contains(search.ToLower())
                        || s.Course.Name.ToLower().Contains(search.ToLower()))
                    .Include(s => s.Course).AsQueryable();
            }
            else
            {
                studentQuery = this._dbContext.Classes.Include(c => c.Course).AsQueryable();
            }
            var model = studentQuery.ToList();
            return View(model: model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            FillDropdownValues();

            return View(new Class());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Class model)
        {
            if (ModelState.IsValid)
            {
                this._dbContext.Classes.Add(model);

                this._dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var classQuery = this._dbContext.Classes.Where(c => c.ID == id)
                .Include(c => c.Professors).Include(c => c.Students).FirstOrDefault();

            ViewBag.News = this._dbContext.News.Where(n => n.ClassId == id).ToList();

            return View(model: classQuery);
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
