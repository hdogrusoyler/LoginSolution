using LoginSolution.LoginProject.Entity;
using LoginSolution.LoginProject.Entity.EfMapping;
using Microsoft.EntityFrameworkCore;

namespace LoginSolution.LoginProject.DataAccess.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new LogMap());
        }
    }
}