using ExpenseControl_ASP.NET.Models;
using ExpenseControl_ASP.NET.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountsTypesRepository accountsTypesRepository;
        private readonly IAccountsRepository accountsRepository;

        public AccountsController(
            IUsersService usersService,
            IAccountsTypesRepository accountsTypesRepository,
            IAccountsRepository accountsRepository)
        {
            this.usersService = usersService;
            this.accountsTypesRepository = accountsTypesRepository;
            this.accountsRepository = accountsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = usersService.GetUserId();
            var model = new CreateAccountViewModel();
            model.AccountsTypes = await GetAccountsTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountViewModel account)
        {
            var userId = usersService.GetUserId();
            var accountType = await accountsTypesRepository.GetById(account.AccountTypeId, userId);

            if (accountType is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                account.AccountsTypes = await GetAccountsTypes(userId);
                return View(account);
            }

            await accountsRepository.Create(account);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> GetAccountsTypes(int userId)
        {
            var accountsTypes = await accountsTypesRepository.GetAccountsTypes(userId);
            return accountsTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
