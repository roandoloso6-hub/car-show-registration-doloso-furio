using System.Reflection;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Registrations;
using Api.Common.Extensions;
using Api.Features.Registrations;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=carshow.db"));
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

builder.Services.AddValidatorsFromAssembly(typeof(IAppDbContext).Assembly);

builder.Services.AddScoped<ICommandHandler<CreateRegistrationCommand, Result<RegistrationResponse>>, CreateRegistrationHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationsQuery, Result<PagedRegistrationsResponse>>, GetRegistrationsHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationByIdQuery, Result<RegistrationResponse>>, GetRegistrationByIdHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationSummaryQuery, Result<RegistrationSummaryResponse>>, GetRegistrationSummaryHandler>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Car Show Registration API";
        document.Info.Description = "Car Show 2027 registration system for AutoVista Events";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.Registrations.AnyAsync())
    {
        var seedTypes = new[] { RegistrationType.Exhibitor, RegistrationType.Competitor, RegistrationType.VIP };
        var seedStatuses = Enum.GetValues<RegistrationStatus>();
        var seed = new List<Registration>();

        for (var i = 1; i <= 30; i++)
        {
            var type = seedTypes[(i - 1) % seedTypes.Length];
            var status = seedStatuses[(i - 1) % seedStatuses.Length];
            var fee = type switch
            {
                RegistrationType.Exhibitor => 1500m,
                RegistrationType.Competitor => 2500m,
                RegistrationType.VIP => 5000m,
                _ => 0m
            };

            var amountPaid = (i % 4) switch
            {
                0 => 0m,
                1 => fee / 2m,
                2 => fee,
                _ => i % 2 == 0 ? 0m : fee
            };

            seed.Add(new Registration
            {
                Id = Guid.NewGuid(),
                RegistrationNumber = $"CS27-{i:D4}",
                RegistrationDate = DateTime.UtcNow.AddMinutes(-i),
                ParticipantName = $"Participant {i:D2}",
                ContactNumber = $"09{100000000 + i:D9}",
                Email = $"participant{i:D2}@example.com",
                Address = $"Address {i:D2}, Sorsogon City",
                CarMakeModel = $"Car Model {i:D2}",
                PlateNumber = $"CS27{i:D3}",
                RegistrationType = type,
                Status = status,
                RegistrationFee = fee,
                AmountPaid = status == RegistrationStatus.Cancelled ? 0m : amountPaid
            });
        }

        dbContext.Registrations.AddRange(seed);
        await dbContext.SaveChangesAsync();
    }
}

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Welcome to Car Show 2027 Registration API. Open /scalar/v1 for API documentation.");
app.MapRegistrationsEndpoints();

app.Run();
