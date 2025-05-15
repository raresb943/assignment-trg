using Dispatcher.Core.Consumers;
using Dispatcher.Core.Interfaces;
using Dispatcher.Core.Models;
using Dispatcher.Core.Services;
using RemoteBrowser.Contracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Polly;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dispatcher API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("Dispatcher.Api"));
});

builder.Services.AddScoped<ITaskRepository, BrowsingTaskRepository>();
builder.Services.AddScoped<IMessageBusService, MessageBusService>();
builder.Services.AddScoped<BrowsingService>();

builder.Services.AddHttpClient<INodeCommunicationService, NodeCommunicationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NodeServiceUrl"] ?? "http://node:80");
});

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BrowsingTaskResultConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitMqSettings["Host"] ?? "localhost";
        var username = rabbitMqSettings["Username"] ?? "guest";
        var password = rabbitMqSettings["Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IMessageBusService, MessageBusService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                app.Logger.LogWarning(exception,
                    "Failed to connect to database (attempt {RetryCount}). Retrying in {TimeSpan}...",
                    retryCount, timeSpan);
            });

    retryPolicy.Execute(() =>
    {
        app.Logger.LogInformation("Ensuring database exists and is up-to-date");
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Database is ready");
    });
}

app.Run();
