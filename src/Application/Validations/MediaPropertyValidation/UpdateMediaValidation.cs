using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.MediaPropertyValidation;

public class UpdateMediaValidation : AbstractValidator<CreateMediaProperty>
{
    public UpdateMediaValidation()
    {
        RuleFor(x => x.PropertyListingId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Prop 0-dan kiçik ola bilməz.");
    }

    private static bool BeValidUrl(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out _);

    private static bool BeValidMediaType(string mediaType)
        => (mediaType ?? "").Trim().ToLowerInvariant() is "image";
}
