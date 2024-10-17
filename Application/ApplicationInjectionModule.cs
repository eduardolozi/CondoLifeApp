using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application {
    public static class ApplicationInjectionModule {
        public static void AddApplicationServices(this IServiceCollection services) {
            services.AddScoped<UserService>();
            services.AddScoped<IEmailService, MailKitService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<VerificationTokenService>();
            services.AddScoped<CondominiumService>();
		}
    }
}
