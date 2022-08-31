namespace ExpenseControl_ASP.NET.Models
{
    public class PaginationAnswer
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public int TotalRecordsQuantity { get; set; }
        public int TotalPageQuantity => (int)Math.Ceiling((double)TotalRecordsQuantity / RecordsPerPage);
        public string BaseURL { get; set; }

    }
    public class PaginationAnswer<T> : PaginationAnswer
    {
        public IEnumerable<T> Elements { get; set; }
    }

}
