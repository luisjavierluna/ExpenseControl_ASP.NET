using AutoMapper;
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
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IMapper mapper;

        public TransactionsController(
            IUsersService usersService,
            IAccountsRepository accountsRepository,
            ICategoriesRepository categoriesRepository,
            ITransactionsRepository transactionsRepository,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
            this.transactionsRepository = transactionsRepository;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var userId = usersService.GetUserId();
            var model = new CreateTransactionViewModel();
            model.Accounts = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel model)
        {
            var userId = usersService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountsRepository.GetById(model.AccountId, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var category = await categoriesRepository.GetById(model.CategoryId, userId);

            if (category is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            model.UserId = userId;

            if (model.OperationTypeId == OperationType.Expense)
            {
                model.Amount *= -1;
            }

            await transactionsRepository.Create(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string urlReturn = null)
        {
            var userId = usersService.GetUserId();
            var transaction = await transactionsRepository.GetById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var model = mapper.Map<UpdateTransactionsViewModel>(transaction);

            model.PreviousAmount = model.Amount;

            if (model.OperationTypeId == OperationType.Expense)
            {
                model.PreviousAmount = model.Amount * -1;
            }

            model.PreviousAccountId = transaction.AccountId;
            model.Categories = await GetCategories(userId, transaction.OperationTypeId);
            model.Accounts = await GetAccounts(userId);
            model.UrlReturn = urlReturn;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTransactionsViewModel model)
        {
            var userId = usersService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountsRepository.GetById(model.AccountId, userId);

            if (account is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var category = await categoriesRepository.GetById(model.CategoryId, userId);

            if (category is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            var transaction = mapper.Map<Transaction>(model);

            if (model.OperationTypeId == OperationType.Expense)
            {
                transaction.Amount *= -1;
            }

            await transactionsRepository.Update(transaction,
                model.PreviousAmount, model.PreviousAccountId);

            if (string.IsNullOrEmpty(model.UrlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(model.UrlReturn);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string urlReturn = null)
        {
            var userId = usersService.GetUserId();

            var transaction = await transactionsRepository.GetById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("ElementNotFound", "Home");
            }

            await transactionsRepository.Delete(id);

            if (string.IsNullOrEmpty(urlReturn))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlReturn);
            }

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
