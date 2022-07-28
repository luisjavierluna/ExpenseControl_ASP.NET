using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly ITransactionsRepository transactionsRepository;

        public AccountsController(
            IUsersService usersService,
            IAccountsTypesRepository accountsTypesRepository,
            IAccountsRepository accountsRepository,
            IMapper mapper,
            ITransactionsRepository transactionsRepository)
        {
            this.usersService = usersService;
            this.accountsTypesRepository = accountsTypesRepository;
            this.accountsRepository = accountsRepository;
            this.mapper = mapper;
            this.transactionsRepository = transactionsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = usersService.GetUserId();
            var accountsWithAccountType = await accountsRepository.Search(userId);

            var model = accountsWithAccountType
                .GroupBy(x => x.AccountType)
                .Select(group => new AccountsIndexViewModel
                {
                    AccountType = group.Key,
                    Accounts = group.AsEnumerable()
                }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Detail(int id, int month, int year)
        {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            DateTime dateStart;
            DateTime dateEnd;

            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                dateStart = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                dateStart = new DateTime(year, month, 1);
            }

            dateEnd = dateStart.AddMonths(1).AddDays(-1);

            var getTransactionsByAccount = new GetTransactionsByAccount()
            {
                AccountId = id,
                UserId = userId,
                DateStart = dateStart,
                DateEnd = dateEnd
            };

            var transactions = await transactionsRepository
                .GetByAccountId(getTransactionsByAccount);

            var model = new DetailedTransactionsReport();
            ViewBag.Account = account.Name;

            var transactionsByDate = transactions.OrderByDescending(x => x.TransactionDate)
                .GroupBy(x => x.TransactionDate)
                .Select(group => new DetailedTransactionsReport.TransactionsByDate()
                {
                    TransactionDate = group.Key,
                    Transactions = group.AsEnumerable()
                });

            model.GroupedTransactions = transactionsByDate;
            model.DateStart = dateStart;
            model.DateEnd = dateEnd;

            ViewBag.previousMonth = dateStart.AddMonths(-1).Month;
            ViewBag.previousYear = dateStart.AddMonths(-1).Year;
            ViewBag.laterMonth = dateStart.AddMonths(1).Month;
            ViewBag.laterYear = dateStart.AddMonths(1).Year;

            return View(model);
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

        public async Task<IActionResult> Edit(int id)
        {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var model = mapper.Map<CreateAccountViewModel>(account);

            model.AccountsTypes = await GetAccountsTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateAccountViewModel accountToEdit)
        {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetById(accountToEdit.Id, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var accountType = accountsTypesRepository.GetById(accountToEdit.AccountTypeId, userId);

            if (accountType is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            await accountsRepository.Update(accountToEdit);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = usersService.GetUserId();
            var account = await accountsRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            await accountsRepository.Delete(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> GetAccountsTypes(int userId)
        {
            var accountsTypes = await accountsTypesRepository.GetAccountsTypes(userId);
            return accountsTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
