using ExpenseControl_ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class AccountTypeController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AccountType accountType)
        {
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }
            return View();
        }
    }
}
