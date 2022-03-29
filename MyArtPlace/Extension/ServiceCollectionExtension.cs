using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Services;
using MyArtPlace.Data;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbRepository, ApplicationDbRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }

        public static IServiceCollection AddApplicationDbContexts(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyArtPlaceContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }
}