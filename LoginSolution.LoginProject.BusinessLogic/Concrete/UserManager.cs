using LoginSolution.LoginProject.BusinessLogic.Abstract;
using LoginSolution.LoginProject.DataAccess;
using LoginSolution.LoginProject.Entity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace LoginSolution.LoginProject.BusinessLogic.Concrete
{
    public class UserManager : IUserService
    {
        private IUnitOfWork unitOfWork;

        public UserManager(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public User GetById(int Id)
        {
            User res = new User();
            res = unitOfWork.userDal.Get(c => c.Id == Id);
            return res;
        }

        public List<User> GetAll(int page = 1, int pageSize = 0)
        {
            //int page = 1;
            //int pageSize = 0;
            List<User> res = new List<User>();
            res = unitOfWork.userDal.GetList(null, (qry) => qry.OrderByDescending(x => x.Id), page, pageSize, i => i.Include(c => c.Logs));//i => i.Photos
            return res;
        }

        public User GetByEmail(string Email)
        {
            int page = 1;
            int pageSize = 0;
            User res = new User();
            res = unitOfWork.userDal.GetList(x => x.Email == Email, (qry) => qry.OrderByDescending(x => x.Id), page, pageSize, i => i.Include(c => c.Logs)).First();
            return res;
        }

        public string Add(User entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.userDal.Add(entity);
            return unitOfWork.CommitSaveChanges();
        }

        public string AddUserAndLog(User entity, string Mail, string Password, string Email, string Name)
        {
            unitOfWork.BeginTransaction();
            var resEnt = unitOfWork.userDal.Add(entity);
            Log log = new Log();
            log.Text = "Register Success";
            log.User = resEnt;
            log.Date = DateTime.Now;
            log.Type = LogType.Register;
            unitOfWork.logDal.Add(log);
            var retRes = unitOfWork.CommitSaveChanges();
            if (retRes == "2")
            {
                UserConfirm(Mail, Password, Email, Name, entity.Email, entity.ActivationCode);
            }
            return retRes;
        }

        public void UserConfirm(string Mail, string Password, string Email, string Name, string UserMail, string Code)
        {
            var confirm = new
            {
                NetworkCredentialMail = Mail,
                NetworkCredentialPassword = Password,
                MailAddressEmail = Email,
                MailAddressName = Name,
                UserEmail = UserMail,
                ActivateCode = Code
            };

            SmtpClient client = new SmtpClient("smtp.yandex.com", 587);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(confirm.NetworkCredentialMail, confirm.NetworkCredentialPassword);
            MailAddress from = new MailAddress(confirm.MailAddressEmail, confirm.MailAddressName);
            MailAddress to = new MailAddress(confirm.UserEmail);
            MailMessage message = new MailMessage(from, to);

            message.Body = "<p>Activation Code: " + confirm.ActivateCode + "</p>";
            message.IsBodyHtml = true;
            message.Subject = "Login Project";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            client.Send(message);
        }

        public string UpdateUserAndAddLog(User entity)
        {
            unitOfWork.BeginTransaction();
            var resEnt = unitOfWork.userDal.Update(entity);
            Log log = new Log();
            log.Text = "Activate Success";
            log.User = resEnt;
            log.Date = DateTime.Now;
            log.Type = LogType.Activate;
            entity.Logs.Add(log);
            return unitOfWork.CommitSaveChanges();
        }
        public string Update(User entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.userDal.Update(entity);
            return unitOfWork.CommitSaveChanges();
        }

        public string Delete(User entity)
        {
            unitOfWork.BeginTransaction();
            unitOfWork.userDal.Delete(entity);
            return unitOfWork.CommitSaveChanges();
        }

        public int GetRegisteredUserCount(DateTime startDate, DateTime endDate)
        {
            var result = unitOfWork.logDal.GetList(x => x.Date > startDate && x.Date < endDate && x.Type == LogType.Activate, (qry) => qry.OrderBy(x => x.Id), 1, 0);

            return result.Count;
        }

        public int UnactivatedUserCount()
        {
            var registerResult = unitOfWork.logDal.GetList(x => x.Type == LogType.Register && x.Date >= DateTime.Now.AddDays(-1) , (qry) => qry.OrderBy(x => x.Id), 1, 0);  
            var activateResult = unitOfWork.logDal.GetList(x => x.Type == LogType.Activate && x.Date >= DateTime.Now.AddDays(-1) , (qry) => qry.OrderBy(x => x.Id), 1, 0);  
            
            return registerResult.Count - activateResult.Count;
        }

        public int AverageTimeCompleteActivate(DateTime date)
        {
            var averageResult = unitOfWork.logDal.GetList(x => x.Date.Date == date.Date, (qry) => qry.OrderBy(x => x.Id), 1, 0);
            var logUsers = averageResult.Select(x => x.UserId).Distinct();
            int itemTime = 0;
            var totalItem = 0;
            foreach (var user in logUsers)
            {
                var res = averageResult.Where(x => x.UserId == user).ToList();
                if (res.Count() == 2)
                {
                    totalItem++;
                    itemTime += Convert.ToInt32((res[1].Date - res[0].Date).TotalSeconds);
                }
            }
            return totalItem > 0 ? itemTime / totalItem : 0;
        }
    }
}