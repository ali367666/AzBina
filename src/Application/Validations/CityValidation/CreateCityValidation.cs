using Application.Abstracts.Repositories;
using Application.DTOs.CityDTOs.RequestDTOs;
using FluentValidation;
namespace Application.Validations.CityValidation;

public class CreateCityValidation:AbstractValidator<CreateCityDTOs>
{
    public CreateCityValidation(ICityRepository cityRepository)
    {
        RuleFor(c => c.Name)
            .Cascade(CascadeMode.Continue)
            .NotEmpty().WithMessage("Şəhərin adı boş ola bilməz.")
            .MaximumLength(20).WithMessage("Şəhərin adı 20 simvoldan çox ola bilməz.")
            .Must(name => name == null || name.Trim().Length <= 20)
            .WithMessage("Şəhərin adı (trim olunandan sonra) 20 simvoldan çox ola bilməz.");
    }
}
