using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validations.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Login boş ola bilməz.")
                .MaximumLength(256).WithMessage("Login maksimum 256 simvol ola bilər.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password boş ola bilməz.")
                .MaximumLength(256).WithMessage("Password maksimum 256 simvol ola bilər.");
        }
    }
}
