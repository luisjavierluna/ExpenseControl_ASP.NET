using AutoMapper;
using ExpenseControl_ASP.NET.Models;

namespace ExpenseControl_ASP.NET.Services
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, CreateAccountViewModel>();
            CreateMap<UpdateTransactionsViewModel, Transaction>().ReverseMap();
        }
    }
}
