using API.Handlers;
using API.Hubs;

namespace API {
    public static class ApiInjectionModule {
        public static void AddApiServices(this IServiceCollection services) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddProblemDetails();
            services.AddSignalR();
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddScoped<EmailNotificationHub>();
        }
    }
}
