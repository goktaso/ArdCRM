using ArdCRM.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArdCRM.Data.Context;

public class ArdCrmDbContext : DbContext
{
    public ArdCrmDbContext(DbContextOptions<ArdCrmDbContext> options) : base(options) { }

    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Teklif> Teklifler => Set<Teklif>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Musteri
        modelBuilder.Entity<Musteri>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Ad).IsRequired().HasMaxLength(100);
            e.Property(x => x.FirmaAdi).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.Telefon).HasMaxLength(20);
            e.Property(x => x.VergiNo).HasMaxLength(20);
        });

        // Teklif
        modelBuilder.Entity<Teklif>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TeklifNo).IsRequired().HasMaxLength(20);
            e.Property(x => x.Baslik).IsRequired().HasMaxLength(300);
            e.Property(x => x.Tutar).HasColumnType("decimal(18,2)");
            e.Property(x => x.Para).HasMaxLength(3).HasDefaultValue("TRY");

            e.HasOne(x => x.Musteri)
             .WithMany(x => x.Teklifler)
             .HasForeignKey(x => x.MusteriId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
