using Domain.Models;
using FluentValidation;

namespace Application.Validators {
    public class VotingValidator : AbstractValidator<Voting> {
        public VotingValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("A votação deve ter um título.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A votação deve ter uma descrição.");

            RuleFor(x => x.InitialDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("A data de início da votação deve ser a partir da data de hoje.");

            RuleFor(x => x.FinalDate)
                .LessThanOrEqualTo(DateTime.Today.AddDays(14)).WithMessage("A votação deve ter um prazo máximo de 2 semanas.");

            RuleFor(x => x)
                .Must(x => x.FinalDate >= x.InitialDate.AddDays(1)).WithMessage("A votação deve ter um prazo mínimo de 1 dia");
        }
    }
}
