namespace ExpenseControl_ASP.NET.Services
{
    public interface IUsersService
    {
        int GetUserId();
    }

    public class UsersService: IUsersService
    {
        public int GetUserId()
        {
            return 1;
        }
    }
}
