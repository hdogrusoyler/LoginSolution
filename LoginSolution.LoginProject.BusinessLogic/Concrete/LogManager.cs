using LoginSolution.LoginProject.BusinessLogic.Abstract;
using LoginSolution.LoginProject.DataAccess;
using LoginSolution.LoginProject.Entity;
using Microsoft.EntityFrameworkCore;

namespace LoginSolution.LoginProject.BusinessLogic.Concrete
{
    public class LogManager : ILogService
    {
        private IUnitOfWork unitOfWork;

        public LogManager(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public Log GetById(int Id)
        {
            Log res = new Log();
            res = unitOfWork.logDal.Get(c => c.Id == Id);
            return res;
        }

        public List<Log> GetAll(int page = 1, int pageSize = 0)
        {
            //int page = 1;
            //int pageSize = 0;
            List<Log> res = new List<Log>();
            res = unitOfWork.logDal.GetList(null, (qry) => qry.OrderByDescending(x => x.Id), page, pageSize, i => i.Include(c => c.User));//i => i.Photos
            return res;
        }

        public string Add(Log entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.logDal.Add(entity);
            return unitOfWork.CommitSaveChanges();
        }
        public string Update(Log entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.logDal.Update(entity);
            return unitOfWork.CommitSaveChanges();
        }

        public string Delete(Log entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.logDal.Delete(entity);
            return unitOfWork.CommitSaveChanges();
        }
    }
}