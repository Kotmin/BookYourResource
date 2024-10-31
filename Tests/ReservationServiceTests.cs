using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

// using BookYourResource.Services;

public class ReservationServiceTests
{
    private ReservationService GetServiceWithContext(ApplicationDbContext context) => new ReservationService(context);

    [Fact]
    public async Task Reservation_OverlappingTime_FailsValidation()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase_OverlapFail")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            // Arrange
            context.Reservations.Add(new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                StatusId = 1
            });
            await context.SaveChangesAsync();

            var service = GetServiceWithContext(context);

            // Act
            var isAvailable = await service.IsResourceAvailable("resource-1", DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));

            // Assert
            Assert.False(isAvailable);
        }
    }

    [Fact]
    public async Task Reservation_NonOverlappingTime_PassesValidation()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase_NonOverlapPass")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            // Arrange
            context.Reservations.Add(new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                StatusId = 1
            });
            await context.SaveChangesAsync();

            var service = GetServiceWithContext(context);

            // Act
            var isAvailable = await service.IsResourceAvailable("resource-1", DateTime.Now.AddHours(3), DateTime.Now.AddHours(4));

            // Assert
            Assert.True(isAvailable);
        }
    }
}
