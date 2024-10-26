using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

public class ApplicationDbContext : IdentityDbContext<User, Role, string>
{
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<ResourceAttribute> ResourceAttributes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationStatus> ReservationStatuses { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        string deviceTypeId = Guid.NewGuid().ToString();
        string spaceTypeId = Guid.NewGuid().ToString();
        string animalTypeId = Guid.NewGuid().ToString();


        modelBuilder.Entity<ResourceType>().HasData(
            new ResourceType { Id = deviceTypeId, Name = "Device", Description = "Any electronic device" },
            new ResourceType { Id = spaceTypeId, Name = "Space", Description = "Rooms, halls, etc." },
            new ResourceType { Id = animalTypeId, Name = "Animal", Description = "Animals like the yellow rubber duck" }
        );


        string macbookProId = Guid.NewGuid().ToString();
        string yellowDuckId = Guid.NewGuid().ToString();
        string classroomE211Id = Guid.NewGuid().ToString();


        modelBuilder.Entity<Resource>().HasData(
            new Resource { Id = macbookProId, Name = "MacBook Pro", CodeName = "macbook_pro_1", Details = "Apple MacBook Pro 16 inch", TypeId = deviceTypeId },
            new Resource { Id = yellowDuckId, Name = "Yellow Rubber Duck", CodeName = "the_duck", Details = "A small yellow rubber duck", TypeId = animalTypeId },
            new Resource { Id = classroomE211Id, Name = "Classroom E211", CodeName = "e211", Details = "A classroom with projectors", TypeId = spaceTypeId }
        );

        string capacityId = Guid.NewGuid().ToString();

        modelBuilder.Entity<ResourceAttribute>().HasData(
            new ResourceAttribute { Id = capacityId, Name = "Capacity", Value = "150", ResourceId = classroomE211Id }
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

        modelBuilder.Entity<ResourceType>()
            .HasIndex(rt => rt.Name)
            .IsUnique();
        
        modelBuilder.Entity<Resource>()
            .HasOne(rt => rt.ResourceType)
            .WithMany(r => r.Resources)
            .HasForeignKey(r => r.TypeId)
            .IsRequired();

        modelBuilder.Entity<ResourceAttribute>()
            .HasOne(ra => ra.Resource)
            .WithMany(r => r.ResourceAttributes)
            .HasForeignKey(ra => ra.ResourceId)
            .IsRequired(false);

        modelBuilder.Entity<Resource>()
            .HasIndex(r => r.CodeName)
            .IsUnique();
    

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.UserEntity)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .IsRequired();

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.ResourceEntity)
            .WithMany(res => res.Reservations)
            .HasForeignKey(r => r.ResourceId)
            .IsRequired();

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.StatusEntity) 
            .WithMany(rs => rs.Reservations)
            .HasForeignKey(r => r.StatusId)
            .IsRequired(); 
    }
}
