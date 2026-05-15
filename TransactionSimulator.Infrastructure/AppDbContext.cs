using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TransactionSimulator.Domain.Config;
using TransactionSimulator.Domain.Entities;
using TransactionSimulator.Domain.Interfaces;

namespace TransactionSimulator.Infrastructure
{
    public class AppDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        private readonly AppSettings _settings;

        public AppDbContext(DbContextOptions<AppDbContext> options, IOptions<AppSettings> settings) : base(options) 
        {
            _settings = settings.Value ?? throw new NullReferenceException(nameof(settings));
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Region> Regions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema(_settings.DatabaseConfig.SchemaName);
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TimeZoneId).IsRequired().HasMaxLength(100);

                entity.HasData(
                    new Region { Id = 1, Name = "Israel", TimeZoneId = "Israel Standard Time" },
                    new Region { Id = 2, Name = "Japan", TimeZoneId = "Tokyo Standard Time" },
                    new Region { Id = 3, Name = "USA (Eastern)", TimeZoneId = "Eastern Standard Time" },
                    new Region { Id = 4, Name = "France", TimeZoneId = "Romance Standard Time" }
                );
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SubmittedTime).IsRequired();
                entity.Property(e => e.CreatedAtUtc).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ;
                entity.HasOne(d => d.Region)
                    .WithMany()
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
