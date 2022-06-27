using Dapper;
using ExpenseControl_ASP.NET.Models;
using ExpenseControl_ASP.NET.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class AccountsTypesController : Controller
    {
        private readonly IAccountsTypesRepository accountsTypesRepository;
        private readonly IUsersService usersService;

        public AccountsTypesController(
            IAccountsTypesRepository accountsTypesRepository,
            IUsersService usersService)
        {
            this.accountsTypesRepository = accountsTypesRepository;
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = usersService.GetUserId();
            var accountsTypes = await accountsTypesRepository.GetAccountsTypes(userId);
            return View(accountsTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType)
        {
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }

            accountType.UserId = usersService.GetUserId();

            var accountTypeAlreadyExists = await accountsTypesRepository
                .Exists(accountType.Name, accountType.UserId);

            if (accountTypeAlreadyExists)
            {
                ModelState.AddModelError(nameof(accountType.Name),
                    $"Account type {accountType.Name} already exists");

                return View(accountType);
            }

            await accountsTypesRepository.Create(accountType);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = usersService.GetUserId();
            var accountType = await accountsTypesRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var userId = usersService.GetUserId();
            var accountType = await accountsTypesRepository
                .GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            await accountsTypesRepository.Delete(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = usersService.GetUserId();
            var accountType = await accountsTypesRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountType accountType)
        {
            var userId = usersService.GetUserId();
            var accountTypeExists = await accountsTypesRepository
                .GetById(accountType.Id, userId);

            if (accountTypeExists is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            await accountsTypesRepository.Update(accountType);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CheckAccountTypeAlreadyExists(string name)
        {
            var userId = usersService.GetUserId();
            var accountTypeAlreadyExists = await accountsTypesRepository
                .Exists(name, userId);

            if (accountTypeAlreadyExists)
            {
                return Json($"Account type {name} already exists");
            }

            return Json(true);
        }
    }
}
