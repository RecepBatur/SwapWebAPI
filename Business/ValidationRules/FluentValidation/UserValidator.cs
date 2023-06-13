using Core.Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(expression: u => u.UserName).NotEmpty().WithMessage("Kullanıcı adı boş geçilemez!");
            RuleFor(expression: u => u.FirstName).NotEmpty().WithMessage("Ad boş geçilemez!");
            RuleFor(expression: u => u.LastName).NotEmpty().WithMessage("Soyad boş geçilemez!");
            RuleFor(expression: u => u.PasswordSalt).NotEmpty().WithMessage("Şifre boş geçilemez!");
            RuleFor(expression: u => u.PasswordHash).NotEmpty().NotEmpty().WithMessage("Şifre boş geçilemez!");
            RuleFor(expression: u => u.Email).NotEmpty().WithMessage("Email boş geçilemez!");
            RuleFor(expression: u => u.Phone).NotEmpty().WithMessage("Telefon numarası boş geçilemez!");
            RuleFor(expression: u => u.Address).NotEmpty().WithMessage("Adres boş geçilemez!");

        }
    }
}
