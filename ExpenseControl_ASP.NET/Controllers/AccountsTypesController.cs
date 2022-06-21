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

        public AccountsTypesController(IAccountsTypesRepository accountsTypesRepository)
        {
            this.accountsTypesRepository = accountsTypesRepository;
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

            accountType.UserId = 1;

            var accountTypeAlreadyExists = await accountsTypesRepository
                .Exists(accountType.Name, accountType.UserId);

            if (accountTypeAlreadyExists)
            {
                ModelState.AddModelError(nameof(accountType.Name),
                    $"Account type {accountType.Name} already exists");

                return View(accountType);
            }

            await accountsTypesRepository.Create(accountType);

            return View();
        }
    }
}
