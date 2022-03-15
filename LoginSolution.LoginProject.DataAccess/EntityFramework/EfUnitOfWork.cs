using Solution.Core.DataAccess;
using Solution.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.DataAccess.EntityFramework
{
    public class EfUnitOfWork : IUnitOfWork, IDisposable
    {
        private DataContext context;
        private IDbContextTransaction transaction;
        public IUserRepository userDal { get; set; }
        public ILogRepository logDal { get; set; }

        public EfUnitOfWork(DataContext _context, IUserRepository _userDal, ILogRepository _logDal)
        {
            context = _context;
            userDal = _userDal;
            logDal = _logDal;
        }
        public void BeginTransaction()
        {
            transaction = context.Database.BeginTransaction();
        }
        public string CommitSaveChanges()
        {
            string result = "";
            try
            {
                result = Save().ToString();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                result = String.IsNullOrEmpty(e.InnerException?.Message) ? e.Message : e.InnerException.Message;
                //throw;
            }
            finally
            {
                transaction.Dispose();
                Dispose();
            }
            return result;
        }
        public int Save()
        {
            return context.SaveChanges();
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}