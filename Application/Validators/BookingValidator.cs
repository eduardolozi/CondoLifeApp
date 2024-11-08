using System.Data;
using Domain.Models;
using FluentValidation;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators {
    public class BookingValidator : AbstractValidator<Booking> {
        private readonly CondoLifeContext _dbContext;
        public BookingValidator(CondoLifeContext dbContext)
        {
            _dbContext = dbContext;
            RuleLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(x => x)
                .Must(IsUserAlreadyBookedInSpace).WithMessage("Não é possível fazer outra reserva no mesmo espaço quando já existe uma reserva cadastrada.")
                .Must(IsSpaceAvailable).WithMessage("Espaço indisponível: não é possível fazer a reserva.");
            
            RuleFor(x => x.InitialDate)
                .NotEmpty().WithMessage("A data inicial deve ser informada")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("A reserva deve ser feita a partir da data de hoje.")
                .LessThanOrEqualTo(DateTime.Today.AddMonths(3)).WithMessage("Não foi possível realizar a reserva. As reservas podem ser feitas com até 3 meses de antecedência. Por favor, selecione uma data dentro desse período.")
                .Must(DateTimeMustBeAvailable).WithMessage("Já existe uma reserva para este horário");

            RuleFor(x => x)
                .Must(x => x.FinalDate > x.InitialDate && x.FinalDate < x.InitialDate.AddDays(1)).WithMessage("A data final deve ser maior que a data incial e estar no mesmo dia que a data inicial.");
                
            RuleFor(x => x.Description)
                .Must(NullOrValidDescription).WithMessage("Tamanho máximo da descrição: 500 caracteres.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("O status da reserva não existe");
        }

        private bool IsUserAlreadyBookedInSpace(Booking booking)
        {
            return !_dbContext.Booking.Any(x => x.Id != booking.Id && x.UserId == booking.UserId && x.SpaceId == booking.SpaceId && x.InitialDate >= DateTime.Today);
        }

        private bool IsSpaceAvailable(Booking booking)
        {
            var space = _dbContext.Space.AsNoTracking().First(x => x.Id == booking.SpaceId);
            return space.Availability;
        }

        private bool NullOrValidDescription(string? description)
        {
            if (description == null) return true;

            return description.Length < 500;
        }

        private bool DateTimeMustBeAvailable(DateTime date)
        {
            return !(_dbContext.Booking.Any(x => x.InitialDate <= date && x.FinalDate > date));
        }
    }
}
