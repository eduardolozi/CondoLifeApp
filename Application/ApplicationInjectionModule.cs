using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application {
    public static class ApplicationInjectionModule {
        public static void AddApplicationServices(this IServiceCollection services) {
            services.AddScoped<UserService>();
            services.AddScoped<IEmailService, MailKitService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<VerificationTokenService>();
            services.AddScoped<CondominiumService>();
            services.AddScoped<SpaceService>();
            services.AddScoped<BookingService>();

            services.AddScoped<AbstractValidator<Space>, SpaceValidator>();
            services.AddScoped<AbstractValidator<Booking>, BookingValidator>();
        }
    }
}
