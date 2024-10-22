using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Domain.Validators {
	public class UserValidator : AbstractValidator<User> {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(6, 35);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Length(8, 25);
            RuleFor(x => x.Role).IsInEnum();
            RuleFor(x => x.Apartment).NotEmpty().GreaterThan(100);
            //RuleFor(x => x.Photo).Must(IsPngOrJpg).Must(ExpectedSize);
        }

        bool IsPngOrJpg(IFormFile? photo) {
            return true;
        }
        
        bool ExpectedSize(IFormFile? photo) {
            return true;
        }
    }
}
