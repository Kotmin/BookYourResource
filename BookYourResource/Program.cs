using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// setup sql-lite db connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// setup Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // .AddAPIEndpoints()
    .AddDefaultTokenProviders();

var app = builder.Build();

// app.MapIdentityAPI<User, Role>l

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;

//     var userManager = services.GetRequiredService<UserManager<User>>();
//     var roleManager = services.GetRequiredService<RoleManager<Role>>();

//     await SeedUsersAndRolesAsync(userManager, roleManager);
// }

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    // Zmieniamy uzyskiwanie kontekstu
    var context = services.GetRequiredService<ApplicationDbContext>();

    await SeedUsersAndRolesAsync(userManager, roleManager, context);
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages(); // req for Identity

app.Run();




async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<Role> roleManager, ApplicationDbContext context)
{
    // Is empty?
    if (!await context.Permissions.AnyAsync())
    {
        var permissions = new[]
        {
            new Permission { Id = 0, Name = "All", Description = "All Permissions" },
            new Permission { Id = 1, Name = "Read", Description = "Read Only" },
            new Permission { Id = 2, Name = "Make Reservation", Description = "Create reservations" },
            new Permission { Id = 3, Name = "Delete Reservation", Description = "Delete reservations" }
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }


    var permissionList = await context.Permissions.ToListAsync();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var adminRole = new Role
        {
            Name = "Admin",
            Description = "Administrator role",
            Permissions = permissionList.Where(p => p.Id == 0).ToList() // Admin / like sudo Permissions
        };
        await roleManager.CreateAsync(adminRole);
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        var userRole = new Role
        {
            Name = "User",
            Description = "Regular user role",
            Permissions = permissionList.Where(p => p.Id >= 1 && p.Id <= 3).ToList() // Read, Make reservation, Delete reservation
        };
        await roleManager.CreateAsync(userRole);
    }

    if (userManager.Users.All(u => u.UserName != "admin"))
    {
        var adminUser = new User
        {
            UserName = "admin",
            Email = "admin@example.com",
            DisplayName = "Admin"
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    if (userManager.Users.All(u => u.UserName != "andrzej"))
    {
        var andrzejUser = new User
        {
            UserName = "andrzej",
            Email = "andrzej@example.com",
            DisplayName = "Andrzej"
        };

        var result = await userManager.CreateAsync(andrzejUser, "User123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(andrzejUser, "User");
        }
    }
}
