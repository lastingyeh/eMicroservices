using System.Collections.Generic;
using CommandService.Models;

namespace CommandService.SyncDataService.Grpc
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms(); 
    }
}