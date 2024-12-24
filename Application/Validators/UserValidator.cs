using Domain.Models;
using FluentValidation;
using Infraestructure;
using Microsoft.AspNetCore.Http;

namespace Application.Validators {
	public class UserValidator : AbstractValidator<User> {
        private readonly CondoLifeContext _dbContext;
        public UserValidator(CondoLifeContext dbContext)
        {
            _dbContext = dbContext;
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome deve ser informado")
                .Length(6, 35).WithMessage("O nome deve ter entre 6 e 35 caracteres");
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email deve ser informado")
                .EmailAddress().WithMessage("O formato do email não está correto")
                .Must(EmailNotExists).WithMessage("O email já está em uso");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha deve ser informada")
                .Length(8, 25).WithMessage("A senha deve ter entre 8 e 25 caracteres");
            
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("O papel do usuário informado não existe");

            RuleFor(x => x.Apartment)
                .NotEmpty().WithMessage("O apartamento deve ser informado");
            
            //RuleFor(x => x.Photo).Must(IsPngOrJpg).Must(ExpectedSize);
        }

        bool EmailNotExists(string email)
        {
            return !_dbContext.Users.Any(x => x.Email.ToLower() == email.ToLower());
        }
        
        bool IsPngOrJpg(IFormFile? photo) {
            return true;
        }
        
        bool ExpectedSize(IFormFile? photo) {
            return true;
        }
    }
}
