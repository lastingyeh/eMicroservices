using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            var loggerFactory = serviceScope.ServiceProvider.GetService<ILoggerFactory>();

            SeedData(context, loggerFactory.CreateLogger("SeedData"), isProd);
        }

        private static void SeedData(AppDbContext context, ILogger logger, bool isProd)
        {
            if (isProd)
            {
                logger.LogInformation("---> Attempting to apply migrations ...");

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.LogError("---> Error from migrations...", ex.Message);
                }
            }

            if (!context.Platforms.Any())
            {
                logger.LogInformation("---> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "cloud Native Computing Foundation", Cost = "Free" }
                );

                context.SaveChanges();
            }
            else
            {
                logger.LogInformation("---> Have Data already...");
            }
        }
    }
}