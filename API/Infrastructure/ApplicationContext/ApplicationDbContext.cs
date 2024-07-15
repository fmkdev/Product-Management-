using API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace API.Infrastructure.ApplicationContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


            builder.Entity<User>(a =>
            {
                a.ToTable("Users");
            });

            // for sql serve db use NEWGUID() function as default for generating guid

            builder.Entity<User>().Property(a => a.Id);

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens");
            });

            builder.Entity<Role>(a => a.ToTable("Roles"));

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });



            builder.Entity<User>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<User>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            builder.Entity<User>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            builder.Entity<Role>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<Role>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            builder.Entity<Role>().HasData(
            new Role { Id = "1", Name = "Admin" }
            );


            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            

        }
    }
}
