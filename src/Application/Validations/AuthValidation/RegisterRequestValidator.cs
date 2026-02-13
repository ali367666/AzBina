using Application.DTOs.Auth;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validations.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName boş ola bilməz.")
                .MinimumLength(3).WithMessage("UserName minimum 3 simvol olmalıdır.")
                .MaximumLength(50).WithMessage("UserName maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Email formatı yanlışdır.")
                .MaximumLength(256).WithMessage("Email maksimum 256 simvol ola bilər.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName boş ola bilməz.")
                .MaximumLength(200).WithMessage("FullName maksimum 200 simvol ola bilər.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password boş ola bilməz.")
                .MinimumLength(8).WithMessage("Password minimum 8 simvol olmalıdır.")
                .MaximumLength(100).WithMessage("Password maksimum 100 simvol ola bilər.")
                .Must(ContainDigit).WithMessage("Password ən azı 1 rəqəm (0-9) içərməlidir.")
                .Must(ContainUpper).WithMessage("Password ən azı 1 böyük hərf (A-Z) içərməlidir.")
                .Must(ContainLower).WithMessage("Password ən azı 1 kiçik hərf (a-z) içərməlidir.")
                .Must(ContainSpecial).WithMessage("Password ən azı 1 xüsusi simvol içərməlidir (məs: !@#$%^&*).");
        }

        private static bool ContainDigit(string password) =>
            !string.IsNullOrWhiteSpace(password) && password.Any(char.IsDigit);

        private static bool ContainUpper(string password) =>
            !string.IsNullOrWhiteSpace(password) && password.Any(char.IsUpper);

        private static bool ContainLower(string password) =>
            !string.IsNullOrWhiteSpace(password) && password.Any(char.IsLower);

        private static bool ContainSpecial(string password) =>
            !string.IsNullOrWhiteSpace(password) &&
            Regex.IsMatch(password, @"[^a-zA-Z0-9]");
    }
}

