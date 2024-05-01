using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
{
    if (builder.Environment.IsProduction())
    {
        Console.WriteLine("--> Using SqlServer Db");
        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
        );
    }
    else
    {
        Console.WriteLine("--> Using InMem Db");
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
    }
    builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
    builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
    builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
    builder.Services.AddGrpc();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Console.WriteLine($"--> CommandService endpoint: {builder.Configuration["CommandService"]}");
}

var app = builder.Build();
{
    app.MapControllers();
    app.MapGrpcService<GrpcPlatformService>();
    app.MapGet("/protos/platforms.proto", async context => 
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto")));
    app.UseSwagger();
    app.UseSwaggerUI();
    PrepDb.PrepPopulation(app, app.Environment.IsProduction());
    app.Run();
}

