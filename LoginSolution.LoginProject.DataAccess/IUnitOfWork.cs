using Solution.Core.DataAccess;
using Solution.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.DataAccess
{
    public interface IUnitOfWork
    {
        IUserRepository userDal { get; set; }
        ILogRepository logDal { get; set; }

        void BeginTransaction();
        string CommitSaveChanges();
        int Save();
        void Dispose();
    }
}