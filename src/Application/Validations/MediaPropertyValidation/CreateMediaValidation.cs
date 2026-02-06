using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.MediaPropertyValidation;

public class CreateMediaPropertyValidator : AbstractValidator<CreateMediaProperty>
{
    public CreateMediaPropertyValidator()
    {
        RuleFor(x => x.PropertyListingId)
            .GreaterThan(0)
            .WithMessage("PropertyListingId düzgün deyil.");

        RuleFor(x => x.Files)
            .NotEmpty()
            .WithMessage("Ən azı bir şəkil əlavə edilməlidir.");


    }

    private static bool BeValidUrl(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out _);

    private static bool BeValidMediaType(string mediaType)
        => (mediaType ?? "").Trim().ToLowerInvariant() is "image";
}
