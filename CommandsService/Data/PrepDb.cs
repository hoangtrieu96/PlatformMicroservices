using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

        var iCommandRepo = serviceScope.ServiceProvider.GetService<ICommandRepo>();

        if (grpcClient != null && iCommandRepo != null)
        {
            var platforms = grpcClient.ReturnAllPlatforms();
            SeedData(iCommandRepo, platforms);
        }
        else
        {
            Console.WriteLine($"--> Failed to get Platforms or {nameof(iCommandRepo)}");
        }
    }

    public static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("Seeding new platforms...");

        platforms.ToList().ForEach(p =>
        {
            if (!repo.ExternalPlatformExist(p.ExternalId))
                repo.CreatePlatform(p);
                
            repo.SaveChanges();
        });
    }
}