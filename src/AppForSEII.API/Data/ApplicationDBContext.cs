using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;

namespace AppForSEII.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);


    }


    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<CompraModelo3D> ComprasModelo3D { get; set; }

    public DbSet<Modelo3D> Modelos3D { get; set; }
    public DbSet<LicenciaModelo3D> LicenciasModelo3D { get; set; }
    
    public DbSet<LineaCompraModelo> LineaCompraModelos { get; set; }


    //public DbSet<MetodoPago> MetodoPagos { get; set; }   las enum no necesitan su propio DbSet

    public DbSet<Impresora3D> Impresoras3D { get; set; }
    public DbSet<ReservaImpresora> ReservasImpresora { get; set; }
    public DbSet<LineaReserva> LineasReserva { get; set; }

}