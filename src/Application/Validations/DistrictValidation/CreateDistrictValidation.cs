using Application.Abstracts.Repositories;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.DistrictValidation;

public class CreateDistrictValidation:AbstractValidator<DistrictCreateDTO>
{
    public CreateDistrictValidation()
    {
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("Rayonun adı boş ola bilməz.")
            .MaximumLength(20).WithMessage("Rayonun adı 20 simvoldan çox ola bilməz.");

        RuleFor(d => d.CityId)
            .GreaterThan(0).WithMessage("CityId düzgün deyil.");
    }
}
