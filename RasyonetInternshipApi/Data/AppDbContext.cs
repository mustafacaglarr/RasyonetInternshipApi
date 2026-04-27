using Microsoft.EntityFrameworkCore;
using RasyonetInternshipApi.Models;

namespace RasyonetInternshipApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Stock> Stocks => Set<Stock>();

    public DbSet<StockPriceHistory> StockPriceHistories => Set<StockPriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasIndex(stock => stock.Symbol).IsUnique();
            entity.Property(stock => stock.Symbol).HasMaxLength(10).IsRequired();
            entity.Property(stock => stock.CompanyName).HasMaxLength(150).IsRequired();
            entity.Property(stock => stock.CurrentPrice).HasColumnType("decimal(18,4)");
            entity.Property(stock => stock.PreviousClosePrice).HasColumnType("decimal(18,4)");
            entity.Property(stock => stock.ChangePercent).HasColumnType("decimal(18,4)");
        });

        modelBuilder.Entity<StockPriceHistory>(entity =>
        {
            entity.Property(history => history.Price).HasColumnType("decimal(18,4)");
            entity.HasOne(history => history.Stock)
                .WithMany(stock => stock.PriceHistory)
                .HasForeignKey(history => history.StockId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
