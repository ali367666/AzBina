using Application.DTOs.DistrictDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.DistrictValidation;

public class UpdateDistrictValidation: AbstractValidator<GetByIdDistrict>
{
    public UpdateDistrictValidation()
    {
        RuleFor(d => d.Id)
            .GreaterThan(0).WithMessage("Id düzgün deyil.");
        RuleFor(d => d.Name)
            .Cascade(CascadeMode.Continue)
            .NotEmpty().WithMessage("Rayonun adı boş ola bilməz.")
            .MaximumLength(20).WithMessage("Rayonun adı 20 simvoldan çox ola bilməz.")
            .Must(name => name == null || name.Trim().Length <= 20)
            .WithMessage("Rayonun adı (trim olunandan sonra) 20 simvoldan çox ola bilməz.");
        RuleFor(d => d.CityId)
            .GreaterThan(0).WithMessage("Şəhər Id-si düzgün deyil.");
    }
}
