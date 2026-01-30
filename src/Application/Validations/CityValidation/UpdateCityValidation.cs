using Application.Abstracts.Repositories;
using Application.DTOs.CityDTOs.RequestDTOs;
using FluentValidation;

namespace Application.Validations.CityValidation;

public class UpdateCityValidation:AbstractValidator<CreateCityDTOs>
{
    public UpdateCityValidation(ICityRepository cityRepository)
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Şəhərin adı boş ola bilməz.")
            .MaximumLength(20).WithMessage("Şəhərin adı 20 simvoldan çox ola bilməz.")
            .MustAsync(async (name, ct) =>
            {
                var all = await cityRepository.GetAllAsync(ct);
                return !all.Any(c => c.Name == name.Trim());
            })
            .WithMessage("Bu adda city artıq mövcuddur.");
    }
}
