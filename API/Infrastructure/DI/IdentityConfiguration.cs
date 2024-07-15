using API.Domain.Entities;
using API.Infrastructure.ApplicationContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Infrastructure.DI
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddIdentity<User, Role>(opt =>
            {
                // Password settings.
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationContext.ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var tokensecret = configuration.GetSection("TokenSecret").Value;

            var key = Encoding.ASCII.GetBytes(tokensecret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuer = configuration.GetSection("JwtIssuerOptions.Issuer").Value,
                    ValidAudience = configuration.GetSection("JwtIssuerOptions.Issuer").Value,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            return services;
        }
    }
}
