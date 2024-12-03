
using Application.Interfaces;
using Domain.Exceptions;

namespace Worker.BackgroundServices {
    public class RefreshTokenBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        public RefreshTokenBackgroundService(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var horario = int.Parse(_configuration["HorarioDeExecucao:HorarioDeExecucaoRefreshToken"] 
                ?? throw new ResourceNotFoundException("Não foi encontrado nenhum horário de execução para o RefreshTokenBackgroundService"));
            if (DateTime.UtcNow.Hour == horario) {
                while (!stoppingToken.IsCancellationRequested) {
                    await _authService.DeleteExpiredRefreshTokens();   
                }
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}
