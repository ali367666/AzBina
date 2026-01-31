using Application.Abstracts.Repositories;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.PropertyListingValidation;

public class DeletePropertyListingValidation : AbstractValidator<GetByIdPropertyListing>
{
    public DeletePropertyListingValidation(IPropertyListeningRepository listeningRepository)
    {
        RuleFor(c => c.Id)
             .GreaterThan(0).WithMessage("Id düzgün deyil.");
    }
}
