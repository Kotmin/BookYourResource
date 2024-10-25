using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<ResourceAttribute> ResourceAttributes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationStatus> ReservationStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Generate UUID on insert for string PK fields

        // modelBuilder.Entity<Resource>().Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
        // modelBuilder.Entity<ResourceType>().Property(rt => rt.Id).HasDefaultValueSql("gen_random_uuid()");
        // modelBuilder.Entity<ResourceAttribute>().Property(ra => ra.Id).HasDefaultValueSql("gen_random_uuid()");

        modelBuilder.Entity<Resource>().Property(r => r.Id).HasDefaultValueSql("NEWID()");
        modelBuilder.Entity<ResourceType>().Property(rt => rt.Id).HasDefaultValueSql("NEWID()");
        modelBuilder.Entity<ResourceAttribute>().Property(ra => ra.Id).HasDefaultValueSql("NEWID()");


          modelBuilder.Entity<Resource>()
            .HasOne(rt => rt.ResourceType)
            .WithMany(r => r.Resources)
            .HasForeignKey(r => r.TypeId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Resource)
            .WithMany(res => res.Reservations)
            .HasForeignKey(r => r.ResourceId);

        modelBuilder.Entity<Reservation>()
            .HasOne(rs => rs.ReservationStatus)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.StatusId);

        // Seed data

        modelBuilder.Entity<ResourceType>().HasData(
            new ResourceType { Id = Guid.NewGuid().ToString(), Name = "Device", Description = "Any electronic device" },
            new ResourceType { Id = Guid.NewGuid().ToString(), Name = "Space", Description = "Rooms, halls, etc." },
            new ResourceType { Id = Guid.NewGuid().ToString(), Name = "Animal", Description = "Animals like the yellow rubber duck" }
        );

        modelBuilder.Entity<Resource>().HasData(
            new Resource { Id = Guid.NewGuid().ToString(), Name = "MacBook Pro", CodeName = "macbook_pro_1", Details = "Apple MacBook Pro 16 inch", Type = "1", Attribute = Guid.NewGuid().ToString() },
            new Resource { Id = Guid.NewGuid().ToString(), Name = "Yellow Rubber Duck", CodeName = "the_duck", Details = "A small yellow rubber duck", Type = "3", Attribute = Guid.NewGuid().ToString() },
            new Resource { Id = Guid.NewGuid().ToString(), Name = "Classroom E211", CodeName = "e211", Details = "A classroom with projectors", Type = "2", Attribute = Guid.NewGuid().ToString() }
        );

        modelBuilder.Entity<ReservationStatus>().HasData(
            new ReservationStatus { Id = 1, Name = "Active" },
            new ReservationStatus { Id = 2, Name = "Canceled" }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 0, Name = "All", Description = "All Permissions" },
            new Permission { Id = 1, Name = "Read", Description = "Read Only" },
            new Permission { Id = 2, Name = "Make Reservation", Description = "Create reservations" },
            new Permission { Id = 3, Name = "Delete Reservation", Description = "Delete reservations" }
        );
    }
}
