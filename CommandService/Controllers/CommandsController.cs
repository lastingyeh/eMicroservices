using System.Collections.Generic;
using AutoMapper;
using commandservice.Data;
using commandservice.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace commandservice.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<CommandsController> _logger;
        public CommandsController(ICommandRepo repo, IMapper mapper, ILogger<CommandsController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsByPlatform(int platformId)
        {
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repo.GetCommandsByPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandByPlatform")]
        public ActionResult<CommandReadDto> GetCommandByPlatform(int platformId, int commandId)
        {
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _repo.GetCommand(platformId, commandId);

            if (command is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandByPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);

            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandByPlatform), 
                new { platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}