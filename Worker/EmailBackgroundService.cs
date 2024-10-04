namespace Worker {
    public class EmailBackgroundService : BackgroundService {

        public EmailBackgroundService() {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
            
            }
        }
    }
}
