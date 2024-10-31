// using BookYourResource.Controllers;  
// using BookYourResource.Models;       
// using Microsoft.EntityFrameworkCore;
// using Xunit;

// // For now we're testing unused elsewhere function yey!!!

// public class ReservationAviableTests
// {
//     [Fact]
//     async public void Reservation_OverlappingTime_FailsValidation()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: "TestDatabase")
//             .Options;

//         using (var context = new ApplicationDbContext(options))
//         {
//             // Given
//             var reservation1 = new Reservation
//             {
//                 ResourceId = "resource-1",
//                 UserId = "user-1",
//                 StartDate = DateTime.Now,
//                 EndDate = DateTime.Now.AddHours(2),
//                 StatusId = 1
//             };

//             context.Reservations.Add(reservation1);
//             await context.SaveChangesAsync();

//             var startDate = DateTime.Now.AddHours(1);
//             var endDate = DateTime.Now.AddHours(3);

//             // When
//             var controller = new ReservationsController(context, null,null); // Mock UserManager if necessary
//             var isAvailable = await controller.IsResourceAvailable(reservation1.ResourceId, startDate, endDate);

//             // Assert
//             Assert.False(isAvailable);
//         }
//     }


//     [Fact]
//     async public void Reservation_OverlappingTime_PassValidation()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: "TestDatabase")
//             .Options;

//         using (var context = new ApplicationDbContext(options))
//         {
//             // Given
//             var reservation1 = new Reservation
//             {
//                 ResourceId = "resource-1",
//                 UserId = "user-1",
//                 StartDate = DateTime.Now,
//                 EndDate = DateTime.Now.AddHours(2),
//                 StatusId = 1
//             };

//             context.Reservations.Add(reservation1);
//             await context.SaveChangesAsync();

//             var startDate = DateTime.Now.AddHours(5);
//             var endDate = DateTime.Now.AddHours(6);

//             // When
//             var controller = new ReservationsController(context, null,null); 
//             var isAvailable = await controller.IsResourceAvailable(reservation1.ResourceId, startDate, endDate);

//             // Assert
//             Assert.True(isAvailable);
//         }
//     }

// }



// // using Microsoft.EntityFrameworkCore;
// // // using BookYourResource.Data;
// // // using BookYourResource.Models;
// // using System;
// // using Xunit;

// // // using BookYourResource.Controllers.ReservationController;
// // // using BookYourResource.Controllers;
// // // using BookYourResource.Data;

// // namespace BookYourResource.Tests;

// // public class ReservationTests
// // {



// //     [Fact]
// //     async public void Reservation_OverlappingTime_FailsValidation()
// //     {
// //         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
// //             .UseInMemoryDatabase(databaseName: "TestDatabase")
// //             .Options;

// //         using (var context = new ApplicationDbContext(options))
// //         {   // given
// //             var reservation1 = new Reservation
// //             {
// //                 ResourceId = "resource-1",
// //                 UserId = "user-1",
// //                 StartDate = DateTime.Now,
// //                 EndDate = DateTime.Now.AddHours(2),
// //                 StatusId = 1
// //             };

// //             // var reservation2 = new Reservation
// //             // {
// //             //     ResourceId = "resource-1",
// //             //     UserId = "user-2",
// //             //     StartDate = DateTime.Now.AddHours(1),
// //             //     EndDate = DateTime.Now.AddHours(3),
// //             //     StatusId = 1
// //             // };


// //             // when
// //             context.Reservations.Add(reservation1);
// //             // context.Reservations.Add(reservation2);
// //             await context.SaveChangesAsync();


// //             var startDate = DateTime.Now.AddHours(1);
// //             var endDate = DateTime.Now.AddHours(3);

// //             var isAvailable = await new ReservationController(context, null)
// //                 .IsReservationAvailable(reservation1.ResourceId, startDate, endDate);


// //             // Assert.False(isOverlapping);
// //             Assert.False(isAvailable);
// //         }
// //     }



// // }


//     // [Fact]
//     // async public void Reservation_OverlappingTime_ProperValidation()
//     // {
//     //     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//     //         .UseInMemoryDatabase(databaseName: "TestDatabase")
//     //         .Options;

//     //     using (var context = new ApplicationDbContext(options))
//     //     {   // given
//     //         var reservation1 = new Reservation
//     //         {
//     //             ResourceId = "resource-1",
//     //             UserId = "user-1",
//     //             StartDate = DateTime.Now,
//     //             EndDate = DateTime.Now.AddHours(2),
//     //             StatusId = 1
//     //         };

//     //         // var reservation2 = new Reservation
//     //         // {
//     //         //     ResourceId = "resource-1",
//     //         //     UserId = "user-2",
//     //         //     StartDate = DateTime.Now.AddHours(1),
//     //         //     EndDate = DateTime.Now.AddHours(3),
//     //         //     StatusId = 1
//     //         // };


//     //         // when
//     //         context.Reservations.Add(reservation1);
//     //         // context.Reservations.Add(reservation2);
//     //         context.SaveChanges();

//     //         var isOverlapping = await IsReservationAvailable("resource-1",DateTime.Now.AddHours(5),DateTime.Now.AddHours(7));

//     //         // then
//     //         Assert.True(isOverlapping);
//     //     }
//     // }

//     // [Fact]
//     // public void Reservation_OverlappingTime_FailsValidation()
//     // {
//     //     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//     //         .UseInMemoryDatabase(databaseName: "TestDatabase")
//     //         .Options;

//     //     using (var context = new ApplicationDbContext(options))
//     //     {   // given
//     //         var reservation1 = new Reservation
//     //         {
//     //             ResourceId = "resource-1",
//     //             UserId = "user-1",
//     //             StartDate = DateTime.Now,
//     //             EndDate = DateTime.Now.AddHours(2),
//     //             StatusId = 1
//     //         };

//     //         var reservation2 = new Reservation
//     //         {
//     //             ResourceId = "resource-1",
//     //             UserId = "user-2",
//     //             StartDate = DateTime.Now.AddHours(1),
//     //             EndDate = DateTime.Now.AddHours(3),
//     //             StatusId = 1
//     //         };


//     //         // when
//     //         context.Reservations.Add(reservation1);
//     //         context.Reservations.Add(reservation2);
//     //         context.SaveChanges();


//     //         // then
//     //         // var isOverlapping = context.Reservations.Any(r => 
//     //         //     r.ResourceId == reservation2.ResourceId && 
//     //         //     r.StatusId == 1 && 
//     //         //     reservation2.EndDate > r.StartDate && 
//     //         //     reservation2.StartDate < r.EndDate);

//     //         var isOverlapping = await IsReservationAvailable("resource-1",DateTime.Now.AddHours(5),DateTime.Now.AddHours(7));

//     //         // Assert.False(isOverlapping);
//     //         Assert.True(isOverlapping);
//     //     }
//     // }