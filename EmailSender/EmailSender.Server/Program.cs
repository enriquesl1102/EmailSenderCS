using EmailSender.Server.Models;
using EmailSender.Server.Services;
using EmailSender.Server;
using Microsoft.EntityFrameworkCore;

// ─── 1. BUILDER ─────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args); // Crea el constructor de la aplicación web. Punto de entrada equivalente a main()

builder.AddServiceDefaults(); // Servicios de Aspire: telemetría, health checks, service discovery
builder.Services.AddProblemDetails(); // Manejo estándar de errores HTTP (400, 500, etc.)
builder.Services.AddOpenApi(); // Swagger: interfaz web para probar endpoints en desarrollo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source = EmailSender.db"));

var smtpSettings = builder.Configuration
    .GetSection("SmtpSettings")
    .Get<SmtpSettings>();

builder.Services.AddSingleton(smtpSettings);
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<QueueService>();
builder.Services.AddHostedService<EmailBackgroundService>();


// ─── 2. BUILD ────────────────────────────────────────────────────────────────
// A partir de aquí ya no se registran servicios, solo se configura el pipeline HTTP

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment()) // Expone Swagger solo en entorno de desarrollo

{
    app.MapOpenApi();
}

// ─── 3. ENDPOINTS ────────────────────────────────────────────────────────────
// Recibe una alarma via POST y devuelve confirmación


app.MapPost("/notify/alarm", async (AlarmDto dto, QueueService queueService) =>
{
    try
    {
        await queueService.EnqueueAsync(dto);
        return Results.Ok($"Alarma recibida: {dto.Titulo}, a fecha {dto.Fecha}\n" +
            $" Ubicación: {dto.Ubicacion}\n" +
            $"Sensor: {dto.Sensor}\n " +
            $"Motivo: {dto.Motivo}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR]: {ex.Message}");
        return Results.Problem(ex.Message);
    }

});

app.MapDefaultEndpoints();

// ─── 4. RUN ──────────────────────────────────────────────────────────────────
// Arranca el servidor y se queda escuchando peticiones

app.Run();