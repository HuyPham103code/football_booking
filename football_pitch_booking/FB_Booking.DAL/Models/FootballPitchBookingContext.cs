using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace FB_Booking.DAL.Models
{
    public partial class FootballPitchBookingContext : DbContext
    {
        public FootballPitchBookingContext()
        {
        }

        public FootballPitchBookingContext(DbContextOptions<FootballPitchBookingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<FootballPitch> FootballPitches { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=ADMIN-PC\\HUYPHAM;Initial Catalog=FootballPitchBooking;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.PaymentState).HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK__Bill__BookingId__534D60F1");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.TimeStart).HasColumnType("datetime");

                entity.HasOne(d => d.Pitch)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PitchId)
                    .HasConstraintName("FK__Booking__PitchId__5070F446");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Booking__UserId__4F7CD00D");
            });

            modelBuilder.Entity<FootballPitch>(entity =>
            {
                entity.HasKey(e => e.PitchId)
                    .HasName("PK__Football__8B89B6E6189DCA9E");

                entity.ToTable("FootballPitch");

                entity.Property(e => e.Location).HasMaxLength(200);

                entity.Property(e => e.PitchName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Size).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053443EDB4C4")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__RoleId__4AB81AF0");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
