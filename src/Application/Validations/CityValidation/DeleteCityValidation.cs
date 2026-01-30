using Application.Abstracts.Repositories;
using Application.DTOs.CityDTOs.RequestDTOs;
using FluentValidation;
namespace Application.Validations.CityValidation;

public class DeleteCityValidation:AbstractValidator<GetByIdDTOs>
{
    public DeleteCityValidation()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("Id düzgün deyil.");
            
            
       
    }
}
