using GameOfLife.Interfaces;
using GameOfLife.Repositories;
using GameOfLife.Services;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Services
{ 
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext with the connection string from configuration
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IGameService, GameService>();
        }
    }
}