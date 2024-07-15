using API.Application;
using API.Infrastructure.ApplicationContext;
using API.Infrastructure.Repo;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetSection("DbConnection").Value;


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            return services;
        }
    }
}
