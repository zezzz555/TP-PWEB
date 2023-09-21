using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Rental.Models;
using System.Reflection.Metadata;

namespace Rental.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // para usar application user
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Rental.Models.Veiculo> Veiculo { get; set; }
        public DbSet<Rental.Models.Categoria> Categoria { get; set; }
        public DbSet<Rental.Models.Empresa> Empresa { get; set; }
        public DbSet<Rental.Models.Reserva> Reserva { get; set; }
        public DbSet<Rental.Models.EstadoVeiculo> EstadoVeiculos { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //modelBuilder.Entity<ApplicationUser>()
        //    //    .HasOne(p => p.empresa)
        //    //    .WithMany(b => b.funcionarios)
        //    //    .HasForeignKey(p => p.EmpresaId);

        //    //modelBuilder.Entity<Empresa>()
        //    //            .HasOne(u => u.gestor)
        //    //            .WithOne() 
        //    //            .HasForeignKey<Empresa>(u => u.GestorId);

        //}
    }
}