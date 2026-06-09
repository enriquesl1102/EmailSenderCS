using Microsoft.EntityFrameworkCore;
using EmailSender.Server.Models;

namespace EmailSender.Server;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AlarmQueue> AlarmQueue { get; set; }
}