using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Contexts;

public class PersonaContext(DbContextOptions<PersonaContext> options) : DbContext(options)
{
    public DbSet<Persona> Personas { get; set; } = null!;

    public async Task RemoveAll()
    {
        await Database.ExecuteSqlRawAsync("DELETE FROM personas");
    }
}