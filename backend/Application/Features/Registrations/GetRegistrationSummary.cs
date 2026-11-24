using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Registrations;

public sealed record GetRegistrationSummaryQuery()
    : IQuery<Result<RegistrationSummaryResponse>>;

public sealed record RegistrationSummaryResponse(
    int TotalRegistrations,
    int Confirmed,
    int CheckedIn,
    int Pending,
    int Cancelled,
    decimal TotalFees,
    decimal TotalPayments,
    decimal UncollectedPayments);

public sealed class GetRegistrationSummaryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationSummaryQuery, Result<RegistrationSummaryResponse>>
{
    public async Task<Result<RegistrationSummaryResponse>> HandleAsync(
        GetRegistrationSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var all = await dbContext.Registrations.AsNoTracking().ToListAsync(cancellationToken);
        var active = all.Where(x => x.Status != RegistrationStatus.Cancelled).ToList();

        var totalFees = active.Sum(x => x.RegistrationFee);
        var totalPayments = active.Sum(x => x.AmountPaid);

        var response = new RegistrationSummaryResponse(
            all.Count,
            all.Count(x => x.Status == RegistrationStatus.Confirmed),
            all.Count(x => x.Status == RegistrationStatus.CheckedIn),
            all.Count(x => x.Status == RegistrationStatus.Pending),
            all.Count(x => x.Status == RegistrationStatus.Cancelled),
            totalFees,
            totalPayments,
            totalFees - totalPayments);

        return Result<RegistrationSummaryResponse>.Success(response);
    }
}
