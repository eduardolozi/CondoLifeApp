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
                .Must(IsSpaceAvailable).WithMessage("Espaço indisponível: não é possível fazer a reserva.")
                .Must(InitialDateTimeMustBeAvailable).WithMessage("Já existe uma reserva para este horário")
                .Must(x => x.FinalDate > x.InitialDate).WithMessage("A data final deve ser maior que a data inicial.")
                .Must(x => x.FinalDate < x.InitialDate.AddDays(1)).WithMessage("Os horários da reserva devem ser no mesmo dia.")
                .Must(x => x.FinalDate.Hour is <= 22 and >= 10).WithMessage("O horário da reserva deve finalizar até 22:00");

            RuleFor(x => x.InitialDate)
                .NotEmpty().WithMessage("A data inicial deve ser informada")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("A reserva deve ser feita a partir da data de hoje.")
                .LessThanOrEqualTo(DateTime.Today.AddMonths(3)).WithMessage(
                    "Não foi possível realizar a reserva. As reservas podem ser feitas com até 3 meses de antecedência. Por favor, selecione uma data dentro desse período.")
                .Must(x => x.Hour is >= 10 and <= 22).WithMessage("O horário de reserva é das 10:00 a 22:00");

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

        private bool InitialDateTimeMustBeAvailable(Booking booking)
        {
            return !_dbContext.Booking.Any(x => x.InitialDate <= booking.InitialDate && x.FinalDate > booking.InitialDate && x.SpaceId == booking.SpaceId);
        }
    }
}
