using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.PropertyListingValidation;

public class UpdatePropertyListingValidation:AbstractValidator<CreatePropertyListing>
{
    public UpdatePropertyListingValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Elanın başlığı boş ola bilməz.")
            .MaximumLength(100).WithMessage("Elanın başlığı 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Elanın təsviri boş ola bilməz.")
            .MaximumLength(1000).WithMessage("Elanın təsviri 1000 simvoldan çox ola bilməz.");

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Sahə 0-dan böyük olmalıdır.");

        RuleFor(x => x.Rooms)
            .GreaterThan(0).WithMessage("Otaq sayı 0-dan böyük olmalıdır.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("CityId düzgün deyil.");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).WithMessage("DistrictId düzgün deyil.");

        RuleFor(x => x.ListingType)
            .IsInEnum().WithMessage("ListingType düzgün deyil.");

        RuleFor(x => x.PropertyType)
            .IsInEnum().WithMessage("PropertyType düzgün deyil.");

        RuleFor(x => x.RenovationStatus)
            .IsInEnum().WithMessage("RenovationStatus düzgün deyil.");
    }
}
