using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Registrations;

public sealed record CreateRegistrationCommand(
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    decimal AmountPaid) : ICommand<Result<RegistrationResponse>>;

public sealed class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationValidator()
    {
        RuleFor(x => x.ParticipantName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ContactNumber).NotEmpty().Matches(@"^09\d{9}$");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.CarMakeModel).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(15);
        RuleFor(x => x.RegistrationType).IsInEnum();
        RuleFor(x => x.AmountPaid).GreaterThanOrEqualTo(0m);
    }
}

public sealed class CreateRegistrationHandler(IAppDbContext dbContext)
    : ICommandHandler<CreateRegistrationCommand, Result<RegistrationResponse>>
{
    public async Task<Result<RegistrationResponse>> HandleAsync(
        CreateRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedPlate = command.PlateNumber.Trim().ToUpperInvariant();
        if (await dbContext.Registrations.AsNoTracking()
            .AnyAsync(x => x.PlateNumber.ToUpper() == normalizedPlate, cancellationToken))
        {
            return Result<RegistrationResponse>.Failure(
                new Error("Registration.Conflict", "A registration with this plate number already exists."));
        }

        var fee = command.RegistrationType switch
        {
            RegistrationType.Exhibitor => 1500m,
            RegistrationType.Competitor => 2500m,
            RegistrationType.VIP => 5000m,
            _ => 0m
        };

        if (command.AmountPaid > fee)
        {
            return Result<RegistrationResponse>.Failure(
                Error.Validation("Registration.Validation", "AmountPaid cannot exceed the registration fee."));
        }

        var nextNumber = await dbContext.Registrations
            .AsNoTracking()
            .CountAsync(cancellationToken) + 1;

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = $"CS27-{nextNumber:D4}",
            RegistrationDate = DateTime.UtcNow,
            ParticipantName = command.ParticipantName.Trim(),
            ContactNumber = command.ContactNumber.Trim(),
            Email = command.Email.Trim(),
            Address = command.Address.Trim(),
            CarMakeModel = command.CarMakeModel.Trim(),
            PlateNumber = normalizedPlate,
            RegistrationType = command.RegistrationType,
            Status = RegistrationStatus.Pending,
            RegistrationFee = fee,
            AmountPaid = command.AmountPaid
        };

        dbContext.Registrations.Add(registration);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<RegistrationResponse>.Success(registration.ToResponse());
    }
}
