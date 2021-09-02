using System;
using System.Text.Json;
using AutoMapper;
using commandservice.Data;
using commandservice.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace commandservice.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventProcessor> _logger;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationManager)
        {
            _logger.LogInformation("---> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationManager);

            switch (eventType.Event)
            {
                case "Platform_published":
                    _logger.LogInformation("Platform published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    _logger.LogInformation("---> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);

                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                    _logger.LogInformation("---> Platform added...");
                }
                else
                {
                    _logger.LogInformation("---> Platform already exists...");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("---> Could not add Platform to DB", ex.Message);
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined,
    }
}