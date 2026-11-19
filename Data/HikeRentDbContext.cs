using HikeRent.Models;
using Microsoft.EntityFrameworkCore;

namespace HikeRent.Data
{
    public class HikeRentDbContext : DbContext
    {
        public HikeRentDbContext(DbContextOptions<HikeRentDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Brand> Brands => Set<Brand>();

        public DbSet<GearItem> GearItems => Set<GearItem>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Rental> Rentals => Set<Rental>();

        public DbSet<RentalDetail> RentalDetails => Set<RentalDetail>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<ReturnRecord> ReturnRecords => Set<ReturnRecord>();

        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // unique index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.BrandName)
                .IsUnique();

            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.RentalCode)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PaymentCode)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.RentalId)
                .IsUnique();

            modelBuilder.Entity<ReturnRecord>()
                .HasIndex(r => r.RentalDetailId)
                .IsUnique();

            modelBuilder.Entity<RentalDetail>()
                .HasIndex(r => new
                {
                    r.RentalId,
                    r.GearItemId
                })
                .IsUnique();

            // Relation

            // User - Rental
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - ReturnRecord
            modelBuilder.Entity<ReturnRecord>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReturnRecords)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - ActivityLog
            modelBuilder.Entity<ActivityLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category - GearItem
            modelBuilder.Entity<GearItem>()
                .HasOne(g => g.Category)
                .WithMany(c => c.GearItems)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Brand - GearItem
            modelBuilder.Entity<GearItem>()
                .HasOne(g => g.Brand)
                .WithMany(b => b.GearItems)
                .HasForeignKey(g => g.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer - Rental
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental - Rental Detail
            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.Rental)
                .WithMany(r => r.RentalDetails)
                .HasForeignKey(rd => rd.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Gear Item - Rental Detail
            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.GearItem)
                .WithMany(g => g.RentalDetails)
                .HasForeignKey(rd => rd.GearItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental - Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Rental)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(r => r.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental Detail - Return Record
            modelBuilder.Entity<ReturnRecord>()
                .HasOne(rr => rr.RentalDetail)
                .WithOne(rd => rd.ReturnRecord)
                .HasForeignKey<ReturnRecord>(rr => rr.RentalDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            // Default Value

            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("Staff");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<GearItem>()
                .Property(g => g.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<GearItem>()
                .Property(g => g.TotalStock)
                .HasDefaultValue(0);

            modelBuilder.Entity<GearItem>()
                .Property(g => g.MaintenanceStock)
                .HasDefaultValue(0);

            modelBuilder.Entity<GearItem>()
                .Property(g => g.DamagedStock)
                .HasDefaultValue(0);

            modelBuilder.Entity<Customer>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Rental>()
                .Property(r => r.RentalDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Rental>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Rental>()
                .Property(r => r.Status)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Rental>()
                .Property(r => r.PaymentStatus)
                .HasDefaultValue("Unpaid");

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<ReturnRecord>()
                .Property(r => r.ActualReturnDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<ActivityLog>()
                .Property(a => a.ActivityDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
