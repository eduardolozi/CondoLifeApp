using Domain.Models;
using FluentValidation;
using Infraestructure;

namespace Application.Validators {
    public class SpaceValidator : AbstractValidator<Space> {
        private readonly CondoLifeContext _dbContext;
        public SpaceValidator(CondoLifeContext dbContext)
        {
            _dbContext = dbContext;
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome deve ser preenchido.")
                .Length(4, 25).WithMessage("O nome deve ter entre 4 e 25 caracteres.");
            
            RuleFor(x => x.Photo)
                .Must(IsExpectedExtension).WithMessage("A imagem deve ser JPG ou PNG).")
                .Must(IsExpectedSize).WithMessage("A imagem deve ser menor que 400kb");

            RuleFor(x => x.Availability)
                .NotEmpty().WithMessage("A disponibilidade deve ser preenchida.");
            
            RuleFor(x => x)
                .Must(IsSpaceNameAvailable).WithMessage("O espaço já existe.");
        }

        private bool IsExpectedSize(Photo? photo)
        {
            if(photo is null) return true;
            
            var imageBytes = Convert.FromBase64String(photo.ContentBase64!);
            return imageBytes.Length <= 409600;
        }

        private bool StartsWithHeader(byte[] imageBytes, byte[] headerBytes)
        {
            for (var i = 0; i < headerBytes.Length; i++)
            {
                if(imageBytes[i] != headerBytes[i]) return false;
            }

            return true;
        }

        private bool IsExpectedExtension(Photo? photo)
        {
            if(photo is null) return true;
            
            var jpgHeader = new byte[] { 0xFF, 0xD8 };
            var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var imageBytes = Convert.FromBase64String(photo.ContentBase64!);

            var isJpg = StartsWithHeader(imageBytes, jpgHeader);
            var isPng = StartsWithHeader(imageBytes, pngHeader);
            
            return isPng || isJpg;
        }

        private bool IsSpaceNameAvailable(Space space)
        {
            return !_dbContext.Space.Any(x => x.Name == space.Name && x.CondominiumId == space.CondominiumId);
        }
    }
}
