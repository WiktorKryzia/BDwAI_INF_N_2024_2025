using ChessManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChessManager.Areas.Identity.Data;

public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Match> Matches { get; set; }

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(255);
        builder.Property(x => x.LastName).HasMaxLength(255);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .Property(p => p.Gender)
            .HasConversion(
                v => v.ToString(),
                v => (Gender)Enum.Parse(typeof(Gender), v));

        builder.Entity<Tournament>()
            .Property(u => u.LastUpdated)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("GETDATE()");

        builder.Entity<Player>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.WhitePlayer)
            .WithMany()
            .HasForeignKey(m => m.WhitePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne(m => m.BlackPlayer)
            .WithMany()
            .HasForeignKey(m => m.BlackPlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
