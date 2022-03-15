using LoginSolution.LoginProject.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoginSolution.LoginProject.LoginMVC.Helper
{
    public class SampleContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<DataContext>();
            var connectionString = configuration.GetConnectionString("DataConnection");
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("LoginSolution.LoginProject.LoginMVC"));
            return new DataContext(builder.Options);
        }
    }
}