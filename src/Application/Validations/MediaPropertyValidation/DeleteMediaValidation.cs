using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.MediaPropertyValidation;

public class DeleteMediaValidation
    : AbstractValidator<GetByIdMediaProperty>
{
    public DeleteMediaValidation()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id 0-dan böyük olmalıdır.");
    }
}