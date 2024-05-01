using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
    builder.Services.AddScoped<ICommandRepo, CommandRepo>();
    builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
    builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
    builder.Services.AddControllers();
    builder.Services.AddHostedService<MessageBusSubscriber>();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI();
    PrepDb.PrepPopulation(app);
    app.Run();
}