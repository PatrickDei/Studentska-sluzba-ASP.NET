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
        private RoleManager<IdentityRole> _roleManager;

        public NewsController(StudentManagerDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task<IActionResult> Index(bool? ajaxCall)
        {
            IdentityRole roles = null;
            IQueryable<News> newsQuery;
            var userId = this._userManager.GetUserId(base.User);

            if (userId != null)
            {
                var idOfrole = this._dbContext.UserRoles
                    .Where(u => u.UserId == userId)
                    .Select(u => u.RoleId)
                    .First()
                    .ToString();
                roles = await this._roleManager.FindByIdAsync(idOfrole);
            }

            if(roles != null)
                ViewBag.Manager = roles.Name;

            if (roles != null && roles.Name.Equals("Admin"))
                newsQuery = this._dbContext.News.Include(n => n.Class).AsQueryable();
            else
                newsQuery = this._dbContext.News.Where(n => n.EndDate.Date >= DateTime.Now.Date && n.StartDate.Date <= DateTime.Now.Date).Include(n => n.Class).AsQueryable();

            var model = newsQuery.ToList();

            if (ajaxCall != null)
                return PartialView("_IndexTable", model);

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

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var news = this._dbContext.News.First(n => n.ID == id);

            this._dbContext.News.Remove(news);

            this._dbContext.SaveChanges();

            var model = this._dbContext.News.Include(n => n.Class).ToList();

            return RedirectToAction("Index", new { ajaxCall = true});
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
