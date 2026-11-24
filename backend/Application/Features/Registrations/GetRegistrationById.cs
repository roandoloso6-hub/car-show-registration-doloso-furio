using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Registrations;

public sealed record GetRegistrationByIdQuery(Guid Id)
    : IQuery<Result<RegistrationResponse>>;

public sealed class GetRegistrationByIdHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationByIdQuery, Result<RegistrationResponse>>
{
    public async Task<Result<RegistrationResponse>> HandleAsync(
        GetRegistrationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var registration = await dbContext.Registrations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        return registration is null
            ? Result<RegistrationResponse>.Failure(
                Error.NotFound("Registration.NotFound", "The requested registration was not found."))
            : Result<RegistrationResponse>.Success(registration.ToResponse());
    }
}
