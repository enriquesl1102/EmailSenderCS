// Creador del código: Enrique Sanz
// Fecha:09/06/2026
namespace EmailSender.Server.Models;

public class AlarmQueue
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public DateTime Fecha { get; set; }
    public string Sensor { get; set; }
    public string Ubicacion { get; set; }
    public string Motivo { get; set; }
    public string Estado { get; set; }
    public DateTime CreadoEn { get; set; }


}