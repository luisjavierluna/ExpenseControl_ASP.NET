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

        public async Task<IActionResult> Index()
        {
            var userId = usersService.GetUserId();
            var categories = await categoriesRepository.GetCategories(userId);
            return View(categories);
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

        public async Task<IActionResult> Edit(int id)
        {
            var userId = usersService.GetUserId();
            var category = await categoriesRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryToEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryToEdit);
            }

            var userId = usersService.GetUserId();
            var category = await categoriesRepository.GetById(categoryToEdit.Id, userId);

            if (category is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            categoryToEdit.UserId= userId;
            await categoriesRepository.Update(categoryToEdit);
            return RedirectToAction("Index");
        }

    }
}
