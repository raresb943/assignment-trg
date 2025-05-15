using MassTransit;
using Node.Core.Interfaces;
using Node.Core.Models;
using Node.Core.Services;
using Node.Service;
using Node.Service.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ContainerSettings>(builder.Configuration.GetSection("ContainerSettings"));
builder.Services.AddHttpClient<IContainerService, DockerContainerService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:2375/");
});

builder.Services.AddHttpClient("ContainerHealthCheck", client =>
{
    var host = builder.Configuration["ContainerSettings:PayloadServiceHost"] ?? "payload";
    var port = builder.Configuration["ContainerSettings:PayloadServicePort"] ?? "3000";
    client.BaseAddress = new Uri($"http://{host}:{port}/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<IPayloadService, DockerPayloadService>(client =>
{
    var host = builder.Configuration["ContainerSettings:PayloadServiceHost"] ?? "payload";
    var port = builder.Configuration["ContainerSettings:PayloadServicePort"] ?? "3000";

    Console.WriteLine($"Configuring payload service URL: http://{host}:{port}/");

    client.BaseAddress = new Uri($"http://{host}:{port}/");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddScoped<IContainerService, DockerContainerService>();
builder.Services.AddScoped<IPayloadService, DockerPayloadService>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BrowsingTaskCommandConsumer>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
