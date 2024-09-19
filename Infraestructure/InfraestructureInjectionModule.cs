using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure {
    public static class InfraestructureInjectionModule {
        public static void AddInfraServices(this IServiceCollection services) {
            services.AddDbContext<CondoLifeContext>();
        }
    }
}
