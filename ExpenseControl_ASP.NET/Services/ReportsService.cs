using ExpenseControl_ASP.NET.Models;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IReportsService
    {
        Task<DetailedTransactionsReport> GetDetailedTransactionReport(int userId, int month, int year, dynamic ViewBag);
        Task<DetailedTransactionsReport> GetDetailedTransactionReportByAccount(int userId, int accountId, int month, int year, dynamic ViewBag);
        Task<IEnumerable<ResultGetPerWeek>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag);
    }

    public class ReportsService : IReportsService
    {
        private readonly ITransactionsRepository transactionsRepository;
        private readonly HttpContext httpContext;

        public ReportsService(
            ITransactionsRepository transactionsRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.transactionsRepository = transactionsRepository;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultGetPerWeek>> GetWeeklyReport(
            int userId,
            int month,
            int year,
            dynamic ViewBag)
        {
            (DateTime dateStart, DateTime dateEnd) = GenerateDateStartAndEnd(month, year);

            var parameter = new GetTransactionsPerUserParameter()
            {
                UserId = userId,
                DateStart = dateStart,
                DateEnd = dateEnd
            };
            
            AssignValuesToViewBag(ViewBag, dateStart);
            var model = await transactionsRepository.GetPerWeek(parameter);
            return model;
        }


        public async Task<DetailedTransactionsReport>
            GetDetailedTransactionReport(
            int userId, 
            int month, 
            int year, 
            dynamic ViewBag)
        {
            (DateTime dateStart, DateTime dateEnd) = GenerateDateStartAndEnd(month, year);

            var parameter = new GetTransactionsPerUserParameter()
            {
                UserId = userId,
                DateStart = dateStart,
                DateEnd = dateEnd
            };

            var transactions = await transactionsRepository
                .GetByUserId(parameter);

            var model = GenerateDetailedTransactionsReport(dateStart, dateEnd, transactions);
            AssignValuesToViewBag(ViewBag, dateStart);
            return model;
        }

        public async Task<DetailedTransactionsReport>
            GetDetailedTransactionReportByAccount(
            int userId, 
            int accountId, 
            int month, 
            int year, 
            dynamic ViewBag)
        {
            (DateTime dateStart, DateTime dateEnd) = GenerateDateStartAndEnd(month, year);

            var getTransactionsByAccount = new GetTransactionsByAccount()
            {
                AccountId = accountId,
                UserId = userId,
                DateStart = dateStart,
                DateEnd = dateEnd
            };

            var transactions = await transactionsRepository
                .GetByAccountId(getTransactionsByAccount);

            var model = GenerateDetailedTransactionsReport(dateStart, dateEnd, transactions);
            AssignValuesToViewBag(ViewBag, dateStart);
            return model;
        }

        private void AssignValuesToViewBag(dynamic ViewBag, DateTime dateStart)
        {
            ViewBag.previousMonth = dateStart.AddMonths(-1).Month;
            ViewBag.previousYear = dateStart.AddMonths(-1).Year;
            ViewBag.laterMonth = dateStart.AddMonths(1).Month;
            ViewBag.laterYear = dateStart.AddMonths(1).Year;
            ViewBag.urlReturn = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static DetailedTransactionsReport GenerateDetailedTransactionsReport(DateTime dateStart, DateTime dateEnd, IEnumerable<Transaction> transactions)
        {
            var model = new DetailedTransactionsReport();

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
            return model;
        }

        private (DateTime dateStart, DateTime dateEnd) GenerateDateStartAndEnd(int month, int year)
        {
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

            return (dateStart, dateEnd);
        }
    }
}
