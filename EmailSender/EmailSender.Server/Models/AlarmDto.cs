// Creador del código: Enrique Sanz
// Fecha:08/06/2026
namespace EmailSender.Server.Models;

public class AlarmDto
{
    public string Titulo { get; set; }
    public DateTime Fecha { get; set; }
    public string Sensor { get; set; }
    public string Ubicacion { get; set; }
    public string Motivo { get; set; }


}