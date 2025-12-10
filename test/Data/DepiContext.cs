using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using test.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace test.Data;

public partial class DepiContext : IdentityDbContext<ApplicationUser>
{
    public DepiContext()
    {
    }

    public DepiContext(DbContextOptions<DepiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }
    public virtual DbSet<UserConnections> UserConnections { get; set; }
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }
    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<Transactions> Transactions { get; set; }
    public virtual DbSet<PaymentMethods> PaymentMethods { get; set; }
    public virtual DbSet<Orders> Orders { get; set; }


    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }


    public virtual DbSet<VaccinationNeeded> VaccinationNeededs { get; set; }
    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 3. Call the base method FIRST ---
        // This is the most critical change. It configures all the Identity tables correctly.
        base.OnModelCreating(modelBuilder);

        // --- 4. Keep your custom entity configurations ---
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK__Animals__6874563112ABE582");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Recordid).HasName("PK__medical___D82414B603C3C68E");
            entity.HasOne(d => d.Animal).WithMany(p => p.MedicalRecords)
                .HasConstraintName("anim_rec_fk")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("PK__Products__2D172D323F6C272B");
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Reqid).HasName("PK__Requests__20C3720149F8AC2F");
            entity.HasOne(d => d.Animal).WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VaccinationNeeded>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vaccinat__3213E83F846723D8");
            entity.HasOne(d => d.MedicalRecord).WithMany(p => p.VaccinationNeededs)
                .HasConstraintName("vac_rec_fk")
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<ChatMessage>(entity =>
        {
           
            entity.HasOne(m => m.Sender)
                  .WithMany()
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.ClientSetNull);


            entity.HasOne(m => m.Receiver)
                  .WithMany()
                  .HasForeignKey(m => m.ReceiverId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}
