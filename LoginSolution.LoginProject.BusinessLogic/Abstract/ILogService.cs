using LoginSolution.LoginProject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.BusinessLogic.Abstract
{
    public interface ILogService
    {
        Log GetById(int Id);
        List<Log> GetAll(int page = 1, int pageSize = 0);
        string Add(Log entity);
        string Update(Log entity);
        string Delete(Log entity);
    }
}