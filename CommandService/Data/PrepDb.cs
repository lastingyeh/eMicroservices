using System;
using System.Collections.Generic;
using commandservice.Data;
using CommandService.Models;
using CommandService.SyncDataService.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

            var serviceProvider = serviceScope.ServiceProvider;

            SeedData(serviceProvider);
        }

        private static void SeedData(IServiceProvider provider)
        {
            var grpcClient = provider.GetService<IPlatformDataClient>();
            var logger = provider.GetService<ILoggerFactory>().CreateLogger("SeedData");
            var repo = provider.GetService<ICommandRepo>();

            logger.LogInformation("---> Seeding new Platforms...");

            foreach (var platform in grpcClient.ReturnAllPlatforms())
            {
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                }
                repo.SaveChanges();
            }
        }
    }
}