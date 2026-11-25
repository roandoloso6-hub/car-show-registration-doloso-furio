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
        var seedStatuses = new[]
        {
            RegistrationStatus.Pending, RegistrationStatus.Confirmed,
            RegistrationStatus.CheckedIn, RegistrationStatus.Cancelled
        };

        var participantNames = new[]
        {
            "Juan Dela Cruz", "Maria Santos", "Jose Ramirez", "Angelica Reyes",
            "Mark Anthony Cruz", "Jessa Mae Bonifacio", "Ramil Ocampo", "Kathleen Navarro",
            "Carlo Mendoza", "Lyka Garcia", "Paolo Villanueva", "Mae Christine Flores",
            "Alfredo Perez", "Christine Joy Morales", "Ernesto Dominguez", "Lovely Ann Ramos",
            "Miguel Antonio Garcia", "Kristine Mae Bautista", "Danilo Mercado", "Patricia Isabel Lim",
            "Arvin Castillo", "Rowena Pascual", "Gabriel Lorenzo", "Janelle Camille Torres",
            "Francis David Aquino", "Maricel Manalo", "Neil Patrick Santos", "Hazel Marie Gonzales",
            "Ronald Velasco", "Andrea Nicole Rivera"
        };

        var contactNumbers = new[]
        {
            "09171234501", "09181234502", "09201234503", "09171234504", "09281234505",
            "09191234506", "09351234507", "09471234508", "09151234509", "09271234510",
            "09161234511", "09361234512", "09451234513", "09221234514", "09171234515",
            "09321234516", "09281234517", "09191234518", "09461234519", "09201234520",
            "09171234521", "09331234522", "09251234523", "09181234524", "09421234525",
            "09161234526", "09371234527", "09221234528", "09151234529", "09291234530"
        };

        var addresses = new[]
        {
            "123 Rizal Street, Sorsogon City, Sorsogon", "45 Magsaysay Avenue, Naga City, Camarines Sur",
            "78 Peñaranda Street, Legazpi City, Albay", "12 Maharlika Highway, Casiguran, Sorsogon",
            "56 Delfin Rosales Avenue, Daraga, Albay", "88 Lacson Street, Iriga City, Camarines Sur",
            "21 Quezon Avenue, Sorsogon City, Sorsogon", "34 Panganiban Drive, Naga City, Camarines Sur",
            "67 Washington Drive, Legazpi City, Albay", "102 Real Street, Ligao City, Albay",
            "8 Burgos Street, Pilar, Sorsogon", "29 Elias Angeles Street, Naga City, Camarines Sur",
            "51 Rizal Avenue, Tabaco City, Albay", "14 Bonifacio Street, Bulan, Sorsogon",
            "73 Roxas Avenue, Legazpi City, Albay", "26 Peñafrancia Avenue, Naga City, Camarines Sur",
            "95 Magallanes Street, Gubat, Sorsogon", "40 J. Hernandez Avenue, Iriga City, Camarines Sur",
            "18 Bitano Road, Legazpi City, Albay", "63 Maharlika Highway, Donsol, Sorsogon",
            "27 Diversion Road, Camalig, Albay", "82 Bicol Road, Canaman, Camarines Sur",
            "15 Osmeña Street, Sorsogon City, Sorsogon", "49 Landco Business Park, Legazpi City, Albay",
            "33 Tagas Road, Daraga, Albay", "11 Mabini Street, Naga City, Camarines Sur",
            "70 Balogo Road, Sorsogon City, Sorsogon", "24 Lapu-Lapu Street, Guinobatan, Albay",
            "38 Marsella Street, Baao, Camarines Sur", "57 Victory Village, Legazpi City, Albay"
        };

        var carMakeModels = new[]
        {
            "Toyota Supra MK4", "Honda Civic Type R", "Toyota GR Yaris", "Mazda MX-5 Miata",
            "Mitsubishi Lancer Evolution X", "Nissan GT-R R35", "Ford Mustang GT", "Honda Accord 2.0",
            "Toyota Fortuner GR Sport", "Nissan Navara Calibre", "Honda Civic VTi", "Toyota Corolla Altis",
            "Mitsubishi Montero Sport", "Subaru WRX STI", "Nissan Patrol", "Toyota Hilux GR Sport",
            "Mazda3 SkyActiv", "Honda CR-V", "Toyota Camry", "Hyundai Tucson",
            "Kia Carnival", "Mitsubishi Xpander Cross", "Ford Ranger Raptor", "Isuzu mu-X",
            "BMW 320i", "Mercedes-Benz C180", "Toyota Land Cruiser Prado", "Suzuki Jimny",
            "Chevrolet Camaro SS", "Porsche 911 Carrera"
        };

        var seed = new List<Registration>();
        var seedStart = new DateTime(2026, 11, 24, 12, 0, 0, DateTimeKind.Utc);

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

            var amountPaid = (i - 1) % 3 switch
            {
                0 => 0m,
                1 => fee / 2m,
                _ => fee
            };

            var nameForEmail = participantNames[i - 1].ToLowerInvariant()
                .Replace(" ", "")
                .Replace("ñ", "n")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");

            seed.Add(new Registration
            {
                Id = Guid.NewGuid(),
                RegistrationNumber = $"CS27-{i:D4}",
                RegistrationDate = seedStart.AddMinutes(-(31 - i)),
                ParticipantName = participantNames[i - 1],
                ContactNumber = contactNumbers[i - 1],
                Email = $"{nameForEmail}{i:D2}@gmail.com",
                Address = addresses[i - 1],
                CarMakeModel = carMakeModels[i - 1],
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
