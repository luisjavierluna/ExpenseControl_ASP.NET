using AutoMapper;
using ClosedXML.Excel;
using ExpenseControl_ASP.NET.Models;
using ExpenseControl_ASP.NET.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ExpenseControl_ASP.NET.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountsRepository accountsRepository;
        private readonly ICategoriesRepository categoriesRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IMapper mapper;
        private readonly IReportsService reportsService;

        public TransactionsController(
            IUsersService usersService,
            IAccountsRepository accountsRepository,
            ICategoriesRepository categoriesRepository,
            ITransactionsRepository transactionsRepository,
            IMapper mapper,
            IReportsService reportsService)
        {
            this.usersService = usersService;
            this.accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
            this.transactionsRepository = transactionsRepository;
            this.mapper = mapper;
            this.reportsService = reportsService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = usersService.GetUserId();

            var model = await reportsService
                .GetDetailedTransactionReport(userId, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = usersService.GetUserId();
            IEnumerable<ResultGetPerWeek> transactionsPerWeek = await reportsService
                .GetWeeklyReport(userId, month, year, ViewBag);

            var grouped = transactionsPerWeek.GroupBy(x => x.Week).Select(x =>
                new ResultGetPerWeek()
                {
                    Week = x.Key,
                    Incomes = x.Where(x => x.OperationTypeId == OperationType.Income)
                        .Select(x => x.Amount).FirstOrDefault(),
                    Expenses = x.Where(x => x.OperationTypeId == OperationType.Expense)
                        .Select(x => x.Amount).FirstOrDefault()
                }).ToList();

            if (year == 0 || month == 0)
            {
                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var referenceDate
                = new DateTime(year, month, 1);
            var monthDays = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var segmentedDays = monthDays.Chunk(7).ToList();

            for (int i = 0; i < segmentedDays.Count(); i++)
            {
                var week = i + 1;
                var dateStart = new DateTime(year, month, segmentedDays[i].First());
                var dateEnd = new DateTime(year, month, segmentedDays[i].Last());
                var weekGroup = grouped.FirstOrDefault(x => x.Week == week);

                if (weekGroup is null)
                {
                    grouped.Add(new ResultGetPerWeek()
                    {
                        Week = week,
                        DateStart = dateStart,
                        DateEnd = dateEnd
                    });
                }
                else
                {
                    weekGroup.DateStart = dateStart;
                    weekGroup.DateEnd = dateEnd;
                }
            }

            grouped = grouped.OrderByDescending(x => x.Week).ToList();

            var model = new WeeklyReportViewModel();
            model.TransactionsPerWeek = grouped;
            model.ReferenceDate = referenceDate;

            return View(model);
        }

        public async Task<IActionResult> Monthly(int year)
        {
            var userId = usersService.GetUserId();

            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            var transactionsPerMonth = await transactionsRepository.GetPerMonth(userId, year);

            var groupedTransactions = transactionsPerMonth.GroupBy(x => x.Month)
                .Select(x => new ResultGetPerMonth()
                {
                    Month = x.Key,
                    Income = x.Where(x => x.OperationTypeId == OperationType.Income)
                        .Select(x => x.Amount).FirstOrDefault(),
                    Expense = x.Where(x => x.OperationTypeId == OperationType.Expense)
                        .Select(x => x.Amount).FirstOrDefault()
                }).ToList();

            for (int month = 1; month <= 12; month++)
            {
                var transaction = groupedTransactions.FirstOrDefault(x => x.Month == month);
                var referenceDate = new DateTime(year, month, 1);
                if (transaction is null)
                {
                    groupedTransactions.Add(new ResultGetPerMonth()
                    {
                        Month = month,
                        ReferenceDate = referenceDate
                    });
                }
                else
                {
                    transaction.ReferenceDate = referenceDate;
                }
            }

            groupedTransactions = groupedTransactions.OrderByDescending(x => x.Month).ToList();

            var model = new MonthlyReportViewModel();
            model.Year = year;
            model.TransactionsPerMonth = groupedTransactions;

            return View(model);
        }


        public IActionResult ExcelReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelPerMonth(int month, int year)
        {
            var dateStart = new DateTime(year, month, 1);
            var dateEnd = dateStart.AddMonths(1).AddDays(-1);
            var userId = usersService.GetUserId();

            var transactions = await transactionsRepository.GetByUserId(
                new GetTransactionsPerUserParameter
                {
                    UserId = userId,
                    DateStart = dateStart,
                    DateEnd = dateEnd
                });

            var fileName = $"Expense Control - {dateStart.ToString("MMM yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelPerYear(int year)
        {
            var dateStart = new DateTime(year, 1, 1);
            var dateEnd = dateStart.AddYears(1).AddDays(-1);
            var userId = usersService.GetUserId();

            var transactions = await transactionsRepository.GetByUserId(
                new GetTransactionsPerUserParameter
                {
                    UserId = userId,
                    DateStart = dateStart,
                    DateEnd = dateEnd
                });

            var fileName = $"Expense Controller - {dateStart.ToString("yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelAll()
        {
            var dateStart = DateTime.Today.AddYears(-100);
            var dateEnd = DateTime.Today.AddYears(1000);
            var userId = usersService.GetUserId();

            var transactions = await transactionsRepository.GetByUserId(
                new GetTransactionsPerUserParameter
                {
                    UserId = userId,
                    DateStart = dateStart,
                    DateEnd = dateEnd
                });

            var fileName = $"Expense Control - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        private FileResult GenerateExcel(string fileName,
            IEnumerable<Transaction> transactions)
        {
            DataTable dataTable = new DataTable("Transactions");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                    new DataColumn("Data"),
                    new DataColumn("Account"),
                    new DataColumn("Category"),
                    new DataColumn("Note"),
                    new DataColumn("Amount"),
                    new DataColumn("Income/Expense"),
            });

            foreach (var transaction in transactions)
            {
                dataTable.Rows.Add(
                    transaction.TransactionDate,
                    transaction.Account,
                    transaction.Category,
                    transaction.Note,
                    transaction.Amount,
                    transaction.OperationTypeId);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetCalendarTransactions(DateTime start, DateTime end)
        {
            var userId = usersService.GetUserId();
            var transactions = await transactionsRepository.GetByUserId(
                new GetTransactionsPerUserParameter
                {
                    UserId = userId,
                    DateStart = start,
                    DateEnd = end
                });

            var calendarEvents = transactions.Select(transaction => new CalendarEvent()
            {
                Title = transaction.Amount.ToString("N"),
                Start = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                End = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                Color = (transaction.OperationTypeId == OperationType.Expense) ? "Red" : null
            });

            return Json(calendarEvents);
        }

        public async Task<JsonResult> GetTransactionsByDate(DateTime date)
        {
            var userId = usersService.GetUserId();

            var transactions = await transactionsRepository.GetByUserId(
                new GetTransactionsPerUserParameter
                {
                    UserId = userId,
                    DateStart = date,
                    DateEnd = date
                });

            return Json(transactions);
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
