using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.MediaPropertyValidation;

public class UpdateMediaValidation : AbstractValidator<CreateMediaProperty>
{
    public UpdateMediaValidation()
    {
        RuleFor(x => x.MediaUrl)
            .NotEmpty().WithMessage("Media linki boş ola bilməz.")
            .Must(BeValidUrl).WithMessage("Media linki düzgün URL formatında deyil.");

        RuleFor(x => x.MediaType)
            .NotEmpty().WithMessage("Media tipi boş ola bilməz.")
            .Must(BeValidMediaType)
            .WithMessage("Media tipi yalnız 'image' ola bilər.");

        RuleFor(x => x.PropertyListingId)
            .GreaterThan(0).WithMessage("PropertyListingId düzgün deyil.");
    }

    private static bool BeValidUrl(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out _);

    private static bool BeValidMediaType(string mediaType)
        => (mediaType ?? "").Trim().ToLowerInvariant() is "image";
}
