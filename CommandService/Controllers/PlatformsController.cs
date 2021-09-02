using System.Collections.Generic;
using AutoMapper;
using commandservice.Data;
using commandservice.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ILogger<PlatformsController> _logger;
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper, ILogger<PlatformsController> logger)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            _logger.LogInformation("--> Inbound POST # Command Service");

            return Ok("Inbound test of from Platforms Controler");
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
    }
}