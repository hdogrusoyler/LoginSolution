using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginSolution.LoginProject.Entity.EfMapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(e => e.Name)
                   .IsRequired();

            builder.Property(e => e.Email)
                   .IsRequired();

            builder.HasIndex(e => e.Email)
                   .IsUnique();

            builder.HasMany(b => b.Logs)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

             //NoAction //Cascade //Restrict 
        }
    }

    public class LogMap : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.Property(e => e.Text)
                   .IsRequired();

            builder.HasOne(c => c.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
    //dotnet ef migrations add ...
    //dotnet ef database update
}
