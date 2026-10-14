using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EndpointsPage.Entities;

public partial class InternsDbContext : DbContext
{
    public InternsDbContext()
    {
    }

    public InternsDbContext(DbContextOptions<InternsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DomainJdr> DomainJdrs { get; set; }

    public virtual DbSet<GamesJdr> GamesJdrs { get; set; }

    public virtual DbSet<LevelsJdr> LevelsJdrs { get; set; }

    public virtual DbSet<MatchesJdr> MatchesJdrs { get; set; }

    public virtual DbSet<UsersJdr> UsersJdrs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DomainJdr>(entity =>
        {
            entity.HasKey(e => e.DomainId).HasName("PK__Domain_J__2498D7703DD201FC");

            entity.ToTable("Domain_JDR");

            entity.Property(e => e.DomainId).HasColumnName("DomainID");
            entity.Property(e => e.Domain)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GamesJdr>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Games_JD__2AB897FD16F6AF01");

            entity.ToTable("Games_JDR");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.GameName).HasMaxLength(100);
            entity.Property(e => e.GameNameEs).HasMaxLength(200);
            entity.Property(e => e.IconUrl).HasMaxLength(200);
            entity.Property(e => e.ImageUrl).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<LevelsJdr>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Levels_J__09F03C0609821953");

            entity.ToTable("Levels_JDR");

            entity.Property(e => e.LevelId)
                .ValueGeneratedNever()
                .HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
        });

        modelBuilder.Entity<MatchesJdr>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PK__Matches___4218C837CD5EDF2F");

            entity.ToTable("Matches_JDR");

            entity.Property(e => e.MatchId).HasColumnName("MatchID");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.MatchDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TimeSeconds).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Game).WithMany(p => p.MatchesJdrs)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Game");

            entity.HasOne(d => d.Level).WithMany(p => p.MatchesJdrs)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Level");

            entity.HasOne(d => d.User).WithMany(p => p.MatchesJdrs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_User");
        });

        modelBuilder.Entity<UsersJdr>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users_JD__1788CCAC8904CDDA");

            entity.ToTable("Users_JDR");

            entity.HasIndex(e => e.Username, "UQ__Users_JD__536C85E44B64401E").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Adp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ADP");
            entity.Property(e => e.DomainId).HasColumnName("DomainID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Domain).WithMany(p => p.UsersJdrs)
                .HasForeignKey(d => d.DomainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Domain");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
