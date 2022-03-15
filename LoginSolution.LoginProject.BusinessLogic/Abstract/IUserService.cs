using LoginSolution.LoginProject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.BusinessLogic.Abstract
{
    public interface IUserService
    {
        User GetById(int Id);
        User GetByEmail(string Email);
        List<User> GetAll(int page = 1, int pageSize = 0);
        string Add(User entity);
        string AddUserAndLog(User entity, string Mail, string Password, string Email, string Name);
        string UpdateUserAndAddLog(User entity);
        string Update(User entity);
        string Delete(User entity);
        void UserConfirm(string Mail, string Password, string Email, string Name, string UserMail, string Code);
        int GetRegisteredUserCount(DateTime startDate, DateTime endDate);
        int UnactivatedUserCount();
        int AverageTimeCompleteActivate(DateTime date);
    }
}