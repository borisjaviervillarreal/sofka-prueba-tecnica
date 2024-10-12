using CuentaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CuentaService.Infrastructure.Data
{
    public class CuentaDbContext : DbContext
    {
        public CuentaDbContext(DbContextOptions<CuentaDbContext> options) : base(options)
        {
        }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cuenta>()
                .Property(c => c.SaldoInicial)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movimiento>()
                .Property(m => m.Saldo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movimiento>()
                .Property(m => m.Valor)
                .HasPrecision(18, 2);

            // Configurar la relación entre Cuenta y Movimiento
            modelBuilder.Entity<Movimiento>()
                .HasOne<Cuenta>()
                .WithMany()
                .HasForeignKey(m => m.CuentaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
