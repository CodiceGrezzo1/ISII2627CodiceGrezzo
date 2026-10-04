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
    public DbSet<Material> Materiales { get; set; }

    public DbSet<LineaEncargo> LineasEncargo { get; set; }
   
    public DbSet<EncargoImpresion> EncargoImpresiones{ get; set; }

   public DbSet<Pieza3D> Piezas3D { get; set; }
   

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }




}