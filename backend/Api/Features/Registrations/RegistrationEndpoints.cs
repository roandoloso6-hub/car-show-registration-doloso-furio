using Application.Abstractions.Messaging;
using Application.Features.Registrations;
using Api.Common.Extensions;
using Domain.Entities;

namespace Api.Features.Registrations;

public sealed class GetRegistrationsRequest
{
    public RegistrationType? Type { get; init; }
    public RegistrationStatus? Status { get; init; }
    public string? ParticipantName { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public static class RegistrationEndpoints
{
    public static IEndpointRouteBuilder MapRegistrationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/registrations");

        group.MapPost("/", async (
            CreateRegistrationCommand command,
            ICommandHandler<CreateRegistrationCommand, Domain.Common.Result<RegistrationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(command, cancellationToken);
            if (!result.IsSuccess)
                return result.ToProblemDetails();

            var response = result.Value!;
            return Results.Created($"/api/registrations/{response.Id}", response);
        }).AddEndpointFilter<ValidationFilter<CreateRegistrationCommand>>()
          .WithName("CreateRegistration")
          .WithSummary("Create a car show registration");

        group.MapGet("/", async (
            [AsParameters] GetRegistrationsQuery query,
            IQueryHandler<GetRegistrationsQuery, Domain.Common.Result<PagedRegistrationsResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(query, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }).AddEndpointFilter<ValidationFilter<GetRegistrationsQuery>>()
          .WithName("GetRegistrations")
          .WithSummary("Get paged and filtered registrations");

        group.MapGet("/summary", async (
            IQueryHandler<GetRegistrationSummaryQuery, Domain.Common.Result<RegistrationSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetRegistrationSummaryQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }).WithName("GetRegistrationSummary")
          .WithSummary("Get registration summary");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRegistrationByIdQuery, Domain.Common.Result<RegistrationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetRegistrationByIdQuery(id), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }).WithName("GetRegistrationById")
          .WithSummary("Get a registration by ID");

        return app;
    }
}
