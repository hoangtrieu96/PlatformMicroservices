using AutoMapper;
using CommandsService.Models;
using Grpc.Core;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"--> Calling gRPC Service: {_configuration["GrpcPlatform"]}");

        string gRPCAddress = _configuration["GrpcPlatform"]!;
        var channel = GrpcChannel.ForAddress(gRPCAddress);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var response = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(response.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call gRPC Server: {ex.Message}");
            return [];
        }
    }
}