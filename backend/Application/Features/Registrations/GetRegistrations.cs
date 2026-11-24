using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Registrations;

public sealed record GetRegistrationsQuery(
    RegistrationType? Type,
    RegistrationStatus? Status,
    string? ParticipantName,
    int Page = 1,
    int PageSize = 10) : IQuery<Result<PagedRegistrationsResponse>>;

public sealed record PagedRegistrationsResponse(
    IReadOnlyList<RegistrationResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed class GetRegistrationsValidator : AbstractValidator<GetRegistrationsQuery>
{
    public GetRegistrationsValidator()
    {
        RuleFor(x => x.ParticipantName)
            .MaximumLength(150)
            .When(x => x.ParticipantName is not null);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type is not null);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status is not null);
    }
}

public sealed class GetRegistrationsHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationsQuery, Result<PagedRegistrationsResponse>>
{
    public async Task<Result<PagedRegistrationsResponse>> HandleAsync(
        GetRegistrationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var registrations = dbContext.Registrations.AsNoTracking();

        if (query.Type is not null)
            registrations = registrations.Where(x => x.RegistrationType == query.Type.Value);

        if (query.Status is not null)
            registrations = registrations.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.ParticipantName))
        {
            var participantName = query.ParticipantName.Trim().ToLower();
            registrations = registrations.Where(x => x.ParticipantName.ToLower().Contains(participantName));
        }

        var totalCount = await registrations.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var entities = await registrations
            .OrderByDescending(x => x.RegistrationDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = entities.Select(x => x.ToResponse()).ToList();

        return Result<PagedRegistrationsResponse>.Success(
            new PagedRegistrationsResponse(items, query.Page, query.PageSize, totalCount, totalPages));
    }
}
