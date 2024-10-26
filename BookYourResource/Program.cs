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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    await SeedUsersAndRolesAsync(userManager, roleManager);
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


async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
{
    using var scope = roleManager.Context.Database.BeginTransaction();
    var permissionList = roleManager.Context.Set<Permission>().ToList();
    
  
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var adminRole = new Role
        {
            Name = "Admin",
            Description = "Administrator role",
            Permissions = permissionList.Where(p => p.Id == 0).Select(p => p.Id).FirstOrDefault() // Admin / like sudo Permissions
        };
        await roleManager.CreateAsync(adminRole);
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        var userRole = new Role
        {
            Name = "User",
            Description = "Regular user role",
            Permissions = permissionList.Where(p => p.Id >= 1 && p.Id <= 3).Select(p => p.Id).Sum() // Read, Make reservation, Delete reservation
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
    await scope.CommitAsync();
}
