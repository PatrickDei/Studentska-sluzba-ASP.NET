using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentskaSluzba.DAL;
using StudentskaSluzba.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace StudentskaSluzba.Controllers
{
    public class ProfessorController : Controller
    {
        private StudentManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public ProfessorController(StudentManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var professorQuery = this._dbContext.Professors.AsQueryable();

            var model = professorQuery.ToList();
            return View(model: model);
        }

        [HttpPost]
        public IActionResult Index(string search)
        {
            IQueryable<Professor> professorQuery;
            if (search != null)
            {
                professorQuery = this._dbContext.Professors
                    .Where(s =>
                            s.FirstName.ToLower().Contains(search.ToLower())
                            || s.LastName.ToLower().Contains(search.ToLower())
                            || s.DateOfBirth.Year.ToString().Contains(search.ToLower()))
                    .AsQueryable();
            }
            else
            {
                professorQuery = this._dbContext.Professors.AsQueryable();
            }

            var model = professorQuery.ToList();
            return View(model: model);
        }

        public IActionResult Details(int id)
        {
            var professor = this._dbContext.Professors.Include(p => p.Classes).FirstOrDefault(s => s.ID == id);

            return View(model: professor);
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Edit(int id)
        {
            var professor = this._dbContext.Professors.FirstOrDefault(s => s.ID == id);

            return View(model: professor);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> EditClasses(Professor model)
        {
            if (ModelState.IsValid)
            {
                var professor = this._dbContext.Professors.Include(p => p.Classes).FirstOrDefault(c => c.ID == model.ID);
                model.Classes = professor.Classes;

                var ok = await this.TryUpdateModelAsync(professor);

                if (ok && this.ModelState.IsValid)
                {
                    this._dbContext.SaveChanges();
                    List<int> classes = professor.Classes.Select(c => c.ID).ToList();
                    ViewBag.Classes = this._dbContext.Classes.Where(c => classes.Contains(c.ID) == false).ToList();
                    return View(model: professor);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Edit");
            }
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> EditClassesApply(int id, List<int> classID)
        {
            var professor = this._dbContext.Professors.FirstOrDefault(s => s.ID == id);

            ICollection<Class> classes = new Collection<Class>();

            foreach (var c in classID)
                classes.Add(this._dbContext.Classes.FirstOrDefault(a => a.ID == c));

            professor.Classes = classes;

            var ok = await this.TryUpdateModelAsync(professor);

            if (ok)
            {
                this._dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Create()
        {
            return View(new Professor());
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public IActionResult Create(Professor professor)
        {
            this._dbContext.Professors.Add(professor);

            this._dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
