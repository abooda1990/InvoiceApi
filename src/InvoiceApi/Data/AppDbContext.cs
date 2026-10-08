using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserInfo> UserInfos => Set<UserInfo>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceComment> InvoiceComments => Set<InvoiceComment>();
    public DbSet<InvoiceViewAccess> InvoiceViewAccesses => Set<InvoiceViewAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Email).HasMaxLength(200);
            user.Property(u => u.Role).HasConversion<string>();

            user.HasOne(u => u.UserInfo)
                .WithOne()
                .HasForeignKey<User>(u => u.UserInfoId);
        });

        modelBuilder.Entity<UserInfo>(info =>
        {
            info.Property(i => i.FirstName).HasMaxLength(100);
            info.Property(i => i.LastName).HasMaxLength(100);
            info.Property(i => i.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Invoice>(invoice =>
        {
            invoice.Property(i => i.RefId).HasMaxLength(50);
            invoice.Property(i => i.Kid).HasMaxLength(25);
            invoice.Property(i => i.Amount).HasPrecision(18, 2);
            invoice.Property(i => i.Status).HasConversion<string>();

            invoice.HasOne(i => i.User)
                   .WithMany(u => u.Invoices)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceComment>(comment =>
        {
            comment.Property(c => c.Comment).HasMaxLength(1000);

            comment.HasOne(c => c.Invoice)
                   .WithMany(i => i.Comments)
                   .HasForeignKey(c => c.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            comment.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceViewAccess>(access =>
        {
            access.HasKey(a => new { a.InvoiceId, a.UserId });

            access.HasOne(a => a.Invoice)
                  .WithMany(i => i.ViewAccess)
                  .HasForeignKey(a => a.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);

            access.HasOne(a => a.User)
                  .WithMany()
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
