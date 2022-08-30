namespace ExpenseControl_ASP.NET.Models
{
    public class PaginationViewModel
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int maximumNumberRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maximumNumberRecordsPerPage) ?
                    maximumNumberRecordsPerPage : value;
            }
        }

        public int RecordsToAvoid => recordsPerPage * (Page - 1);
    }
}
