using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
    builder.Services.AddScoped<ICommandRepo, CommandRepo>();
    builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
    builder.Services.AddControllers();
    builder.Services.AddHostedService<MessageBusSubscriber>();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Run();
}