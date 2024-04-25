using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
    builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
    builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Console.WriteLine($"--> CommandService endpoint: {builder.Configuration["CommandService"]}");
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI();
    PrepDb.PrepPopulation(app);
    app.Run();
}

