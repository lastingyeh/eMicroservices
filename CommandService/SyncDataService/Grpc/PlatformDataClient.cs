using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlatformService;

namespace CommandService.SyncDataService.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<PlatformDataClient> _logger;
        public PlatformDataClient(IConfiguration config, IMapper mapper, ILogger<PlatformDataClient> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _config = config;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var grpcHost = _config["GrpcPlatform"];

            _logger.LogInformation($"---> Calling Grpc Service {grpcHost}");

            var channel = GrpcChannel.ForAddress(grpcHost);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);

                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                _logger.LogError($"---> Grpc Service error: {ex.Message}");

                return null;
            }
        }
    }
}