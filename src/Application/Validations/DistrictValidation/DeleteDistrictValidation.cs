using Application.DTOs.DistrictDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.DistrictValidation;

public class DeleteDistrictValidation:AbstractValidator<GetByIdDistrict>
{
    public DeleteDistrictValidation()
    {
        RuleFor(d => d.Id)
            .GreaterThan(0).WithMessage("Id düzgün deyil.");
    }
}
