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
    public class NewsController : Controller
    {
        private StudentManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public NewsController(StudentManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            var newsQuery = this._dbContext.News.Where(n => n.EndDate >= DateTime.Now && n.StartDate <= DateTime.Now).Include(n => n.Class).AsQueryable();

            var model = newsQuery.ToList();

            return View(model: model);
        }

        public IActionResult Create()
        {
            FillDropdownValues();

            return View(new News());
        }

        [HttpPost]
        public IActionResult Create(News model)
        {
            if (ModelState.IsValid)
            {
                this._dbContext.News.Add(model);

                this._dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private void FillDropdownValues()
        {
            var selectItems = new List<SelectListItem>();

            foreach (var course in this._dbContext.Classes)
            {
                var listItem = new SelectListItem(course.Name, course.ID.ToString());
                selectItems.Add(listItem);
            }

            ViewBag.PossibleClasses = selectItems;
        }
    }
}
