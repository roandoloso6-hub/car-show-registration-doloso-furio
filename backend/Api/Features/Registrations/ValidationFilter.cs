using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Registrations;

public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (validationResult.IsValid)
            return await next(context);

        var problem = new ValidationProblemDetails(
            validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()))
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        };

        return Results.ValidationProblem(problem.Errors, statusCode: StatusCodes.Status400BadRequest,
            title: problem.Title, detail: "One or more validation errors occurred.");
    }
}
