using ApiTareas.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApiTareas.Data
{
    public class TareasDbContext : DbContext
    {
        public TareasDbContext(DbContextOptions<TareasDbContext> options) : base(options)
        {
        }

        public DbSet<Tarea> Tareas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear los enums como strings en la base de datos para mejor legibilidad e integridad
            modelBuilder.Entity<Tarea>()
                .Property(t => t.Estado)
                .HasConversion(
                    v => v.ToString(),
                    v => (EstadoTarea)Enum.Parse(typeof(EstadoTarea), v));

            modelBuilder.Entity<Tarea>()
                .Property(t => t.Prioridad)
                .HasConversion(
                    v => v.ToString(),
                    v => (PrioridadTarea)Enum.Parse(typeof(PrioridadTarea), v));
        }
    }
}
