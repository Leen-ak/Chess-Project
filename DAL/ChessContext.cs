using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public partial class ChessContext : DbContext
{
    public ChessContext()
    {
    }

    public ChessContext(DbContextOptions<ChessContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=ChessDatabase; Trusted_Connection=True");
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserAccountID");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Password, "IX_Account_Password");

            entity.HasIndex(e => e.Username, "IX_Account_Username");

            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Username).HasMaxLength(60);

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK_UserID_");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Id");

            entity.HasIndex(e => e.FollowerId, "IX_FollowerId");

            entity.HasIndex(e => e.FollowingId, "IX_FollowingId");

            entity.HasIndex(e => e.Status, "IX_Status");

            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "UQ_Follow").IsUnique();

            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.FollowerNavigation).WithMany(p => p.FollowerFollowerNavigations)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK_Follower");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowerFollowings)
                .HasForeignKey(d => d.FollowingId)
                .HasConstraintName("FK_Following");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserID");

            entity.ToTable("UserInfo");

            entity.HasIndex(e => e.Email, "IX_UserInfo_Email").IsUnique();

            entity.HasIndex(e => e.UserName, "IX_UserInfo_UserName").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Firstname).HasMaxLength(60);
            entity.Property(e => e.Lastname).HasMaxLength(60);
            entity.Property(e => e.Password).HasMaxLength(30);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UserName).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
