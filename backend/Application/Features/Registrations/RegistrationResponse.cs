using Domain.Entities;

namespace Application.Features.Registrations;

public sealed record RegistrationResponse(
    Guid Id,
    string RegistrationNumber,
    DateTime RegistrationDate,
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    RegistrationStatus Status,
    decimal RegistrationFee,
    decimal AmountPaid,
    string PaymentStatus);

public static class RegistrationMappings
{
    public static RegistrationResponse ToResponse(this Registration registration) => new(
        registration.Id,
        registration.RegistrationNumber,
        registration.RegistrationDate,
        registration.ParticipantName,
        registration.ContactNumber,
        registration.Email,
        registration.Address,
        registration.CarMakeModel,
        registration.PlateNumber,
        registration.RegistrationType,
        registration.Status,
        registration.RegistrationFee,
        registration.AmountPaid,
        registration.PaymentStatus);
}
