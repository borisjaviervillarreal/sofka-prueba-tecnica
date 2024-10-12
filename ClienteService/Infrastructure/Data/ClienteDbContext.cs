using ClienteService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ClienteService.Infrastructure.Data
{
    public class ClienteDbContext : DbContext
    {
        public ClienteDbContext(DbContextOptions<ClienteDbContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }  // Representa la tabla base que contiene clientes
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la herencia: Persona como tabla base, Cliente como derivada
            modelBuilder.Entity<Cliente>()
                .HasBaseType<Persona>();

            // Configuración de la clave primaria en Persona
            modelBuilder.Entity<Persona>()
                .HasKey(p => p.Id);

            // Configuración para que el campo Id sea Identity (autoincremental)
            modelBuilder.Entity<Persona>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // Configuración para que ClienteId sea único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.ClienteId)
                .IsUnique();
        }
    }
}
