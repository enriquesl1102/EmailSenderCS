using EmailSender.Server.Models;
using MimeKit;
using System.Net.Sockets;
using System.Security.Authentication;


namespace EmailSender.Server.Services;

public class EmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(SmtpSettings settings)
    {
        _settings = settings;
    }
    public async Task SendAsync(AlarmDto alarm)
    {
        // 1. Construir el mensaje
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_settings.Username));
        message.To.Add(MailboxAddress.Parse(_settings.Username));
        message.Subject = $"Alarma: {alarm.Titulo}";
        message.Body = new TextPart("plain")
        {
            Text = $"Sensor: {alarm.Sensor}\n" +
                   $"Ubicación: {alarm.Ubicacion}\n" +
                   $"Fecha: {alarm.Fecha}\n" +
                   $"Motivo: {alarm.Motivo}"
        };

        // 2. Conectar y enviar
        try
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        catch (AuthenticationException ex)
        {
            Console.WriteLine($"[ERROR AUTH]: {ex.Message}.");
            throw;
        }

        catch (SocketException ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}. Es posible que el servidor no esté disponible");
            throw;
        }
    }
}