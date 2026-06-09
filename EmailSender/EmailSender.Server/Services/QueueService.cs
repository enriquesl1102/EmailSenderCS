using EmailSender.Server.Models;
using Microsoft.EntityFrameworkCore;


namespace EmailSender.Server.Services;

public class QueueService
{
    private readonly AppDbContext _context;

    public QueueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task EnqueueAsync(AlarmDto alarm)
    {
        var entry = new AlarmQueue
        {
            Titulo = alarm.Titulo,
            Fecha = alarm.Fecha,
            Sensor = alarm.Sensor,
            Ubicacion = alarm.Ubicacion,
            Motivo = alarm.Motivo,
            Estado = "Pendiente",
            CreadoEn = DateTime.UtcNow
        };

        await _context.AlarmQueue.AddAsync(entry);
        await _context.SaveChangesAsync();
    }
    public async Task<List<AlarmQueue>> GetPendingAsync()
    {
        return await _context.AlarmQueue
            .Where(a => a.Estado == "Pendiente")
            .ToListAsync();
    }
}
