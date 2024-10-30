using Domain.Models;
using FluentValidation;

namespace Domain.Validators {
    public class SpaceValidator : AbstractValidator<Space> {
        public SpaceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome deve ser preenchido.")
                .Length(4, 25).WithMessage("O nome deve ter entre 4 e 25 caracteres.")
                .Must(IsSpaceNameAvailable).WithMessage("O espaço já existe.");

            RuleFor(x => x.Photo)
                .Must(IsExpectedExtension).WithMessage("A imagem deve ser JPG ou PNG).")
                .Must(IsExpectedSize).WithMessage("A imagem deve ser de 300x200 a 1200x800 pixels.");

            RuleFor(x => x.Availability)
                .NotEmpty().WithMessage("A disponibilidade deve ser preenchida.");
        }

        private bool IsExpectedSize(Photo? photo)
        {
            if(photo is null) return true;
            
            return true;
        }

        private bool IsExpectedExtension(Photo? photo)
        {
            if(photo is null) return true;
            
            return true;
        }

        private bool IsSpaceNameAvailable(string name)
        {
            return true;
        }
    }
}
