using ExpenseControl_ASP.NET.Models;
using ExpenseControl_ASP.NET.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoriesRepository categoriesRepository;
        private readonly IUsersService usersService;

        public CategoriesController(
            ICategoriesRepository categoriesRepository,
            IUsersService usersService)
        {
            this.categoriesRepository = categoriesRepository;
            this.usersService = usersService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = usersService.GetUserId();
            category.UserId = userId;
            await categoriesRepository.Create(category);
            return RedirectToAction("Index");
        }
    }
}
