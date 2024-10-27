using Microsoft.EntityFrameworkCore;
// using BookYourResource.Data;
// using BookYourResource.Models;
using System;
using Xunit;

// namespace BookYourResource.Tests;

public class ReservationTests
{
    [Fact]
    public void Reservation_OverlappingTime_FailsValidation()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {   // given
            var reservation1 = new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                StatusId = 1
            };

            var reservation2 = new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-2",
                StartDate = DateTime.Now.AddHours(1),
                EndDate = DateTime.Now.AddHours(3),
                StatusId = 1
            };


            // when
            context.Reservations.Add(reservation1);
            context.Reservations.Add(reservation2);
            context.SaveChanges();


            // then
            // var isOverlapping = context.Reservations.Any(r => 
            //     r.ResourceId == reservation2.ResourceId && 
            //     r.StatusId == 1 && 
            //     reservation2.EndDate > r.StartDate && 
            //     reservation2.StartDate < r.EndDate);

            var isOverlapping = await IsReservationAvailable("resource-1",DateTime.Now.AddHours(1).Now.AddHours(3))

            // Assert.False(isOverlapping);
            Assert.True(isOverlapping);
        }
    }


    [Fact]
    public void Reservation_OverlappingTime_FailsValidation()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {   // given
            var reservation1 = new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                StatusId = 1
            };

            var reservation2 = new Reservation
            {
                ResourceId = "resource-1",
                UserId = "user-2",
                StartDate = DateTime.Now.AddHours(1),
                EndDate = DateTime.Now.AddHours(3),
                StatusId = 1
            };


            // when
            context.Reservations.Add(reservation1);
            context.Reservations.Add(reservation2);
            context.SaveChanges();


            // then
            // var isOverlapping = context.Reservations.Any(r => 
            //     r.ResourceId == reservation2.ResourceId && 
            //     r.StatusId == 1 && 
            //     reservation2.EndDate > r.StartDate && 
            //     reservation2.StartDate < r.EndDate);

            var isOverlapping = await IsReservationAvailable("resource-1",DateTime.Now.AddHours(1).Now.AddHours(3))

            // Assert.False(isOverlapping);
            Assert.True(isOverlapping);
        }
    }

}

