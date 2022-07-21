using ExpenseControl_ASP.NET.Models;
using ExpenseControl_ASP.NET.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountsRepository accountsRepository;
        private readonly ICategoriesRepository categoriesRepository;

        public TransactionsController(
            IUsersService usersService,
            IAccountsRepository accountsRepository,
            ICategoriesRepository categoriesRepository)
        {
            this.usersService = usersService;
            this.accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<IActionResult> Create()
        {
            var userId = usersService.GetUserId();
            var model = new CreateTransactionViewModel();
            model.Accounts = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAccounts(int UserId)
        {
            var cuentas = await accountsRepository.Search(UserId);
            return cuentas.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategories(
            int userId,
            OperationType operationType)
        {
            var categories = await categoriesRepository.GetCategories(userId, operationType);
            return categories.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationType operationType)
        {
            var userId = usersService.GetUserId();
            var categories = await GetCategories(userId, operationType);
            return Ok(categories);
        }


    }
}
