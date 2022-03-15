using Solution.Core.DataAccess.EntityFramework;
using LoginSolution.LoginProject.DataAccess.EntityFramework;
using LoginSolution.LoginProject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.DataAccess.EntityFramework
{
    public class EfUserRepository : EfBaseRepository<User, DataContext>, IUserRepository
    {
        public EfUserRepository(DataContext dbContext) : base(dbContext)
        {

        }
    }
}
