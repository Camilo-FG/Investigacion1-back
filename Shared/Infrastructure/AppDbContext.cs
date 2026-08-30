using Investigacion1_back.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Shared.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<RefreshSession>(entity =>
        {
            entity.HasKey(session => session.Id);
            entity.HasIndex(session => session.TokenHash).IsUnique();
            entity.Property(session => session.TokenHash).IsRequired();
            entity.HasOne(session => session.User)
                .WithMany(user => user.RefreshSessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(room => room.Id);
            entity.HasIndex(room => room.Number).IsUnique();
            entity.Property(room => room.Number).HasMaxLength(16).IsRequired();
            entity.Property(room => room.Type).HasMaxLength(24).HasConversion<string>();
            entity.Property(room => room.Floor).IsRequired();
            entity.Property(room => room.Capacity).IsRequired();
            entity.Property(room => room.BasePricePerNight).IsRequired();
            entity.HasMany(room => room.Reservations)
                .WithOne(reservation => reservation.Room)
                .HasForeignKey(reservation => reservation.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(reservation => reservation.Id);
            entity.Property(reservation => reservation.GuestName).HasMaxLength(128).IsRequired();
            entity.Property(reservation => reservation.CheckInDate).IsRequired();
            entity.Property(reservation => reservation.CheckOutDate).IsRequired();
            entity.Property(reservation => reservation.Guests).IsRequired();
            entity.Property(reservation => reservation.TotalPrice).IsRequired();
            entity.HasOne(reservation => reservation.Room)
                .WithMany(room => room.Reservations)
                .HasForeignKey(reservation => reservation.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
