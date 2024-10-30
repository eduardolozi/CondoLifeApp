using System.Runtime.InteropServices.Marshalling;
using Domain.Models;
using Domain.Utils;
using FluentValidation;

namespace Domain.Validators {
    public class SpaceValidator : AbstractValidator<Space> {
        public SpaceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(25)
                .Must(IsSpaceNameAvailable);

            RuleFor(x => x.Photo)
                .Must(IsExpectedExtension)
                .Must(IsExpectedSize);

            RuleFor(x => x.Availability)
                .NotEmpty();
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
