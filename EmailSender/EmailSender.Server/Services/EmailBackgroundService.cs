using EmailSender.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender.Server.Services;

public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EmailBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPendingAlarmsAsync();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessPendingAlarmsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<QueueService>();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        var pending = await queueService.GetPendingAsync();

        foreach (var alarm in pending)
        {
            try
            {
                await emailService.SendAsync(new AlarmDto
                {
                    Titulo = alarm.Titulo,
                    Fecha = alarm.Fecha,
                    Sensor = alarm.Sensor,
                    Ubicacion = alarm.Ubicacion,
                    Motivo = alarm.Motivo
                });

                alarm.Estado = "Enviado";
            }
            catch (Exception ex)
            {
                alarm.Estado = "Error";
                Console.WriteLine($"[ERROR]: {ex.Message}");
            }
        }

        using var dbScope = _scopeFactory.CreateScope();
        var context = dbScope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.UpdateRange(pending);
        await context.SaveChangesAsync();
    }
}