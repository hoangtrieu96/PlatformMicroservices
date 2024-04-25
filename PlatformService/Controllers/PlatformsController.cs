using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformsController(
        IPlatformRepo platformRepo, 
        IMapper mapper,
        ICommandDataClient commandDataClient)
    {
        _platformRepo = platformRepo;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platformItems = _platformRepo.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platformItem = _platformRepo.GetPlatformById(id);
        if (platformItem != null)
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _platformRepo.CreatePlatform(platformModel);
        _platformRepo.SaveChanges();

        var platformItem = _mapper.Map<PlatformReadDto>(platformModel);

        try
        {
            await _commandDataClient.SendPlatformToCommand(platformItem);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformItem.Id}, platformItem);
    }
}