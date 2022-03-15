using Solution.Core.Entity;

namespace LoginSolution.LoginProject.Entity
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string ActivationCode { get; set; }
        public bool IsActive { get; set; }

        public List<Log> Logs { get; set; }
    }

    public class Log : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public LogType Type { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum LogType
    {
        Register = 1,
        Activate = 2
    }

}