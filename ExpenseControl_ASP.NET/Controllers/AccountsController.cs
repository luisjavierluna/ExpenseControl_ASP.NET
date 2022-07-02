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

        public AccountsController(
            IUsersService usersService,
            IAccountsTypesRepository accountsTypesRepository)
        {
            this.usersService = usersService;
            this.accountsTypesRepository = accountsTypesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = usersService.GetUserId();
            var accountsTypes = await accountsTypesRepository.GetAccountsTypes(userId);
            var model = new CreateAccountViewModel();
            model.AccountsTypes = accountsTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
            return View(model);
        }
    }
}
