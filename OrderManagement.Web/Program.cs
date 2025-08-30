using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Core.Interfaces;
using OrderManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<OrderManagement.Infrastructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<OrderManagement.Infrastructure.Data.AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed Admin and Staff roles and users
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Seed Admin role and user
    string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }
    var adminUser = await userManager.FindByNameAsync("admin@example.com");
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }

    // Seed Staff role and user
    string staffRole = "Staff";
    if (!await roleManager.RoleExistsAsync(staffRole))
    {
        await roleManager.CreateAsync(new IdentityRole(staffRole));
    }
    var staffUser = await userManager.FindByNameAsync("staff@example.com");
    if (staffUser == null)
    {
        staffUser = new IdentityUser { UserName = "staff@example.com", Email = "staff@example.com" };
        await userManager.CreateAsync(staffUser, "Staff@123");
        await userManager.AddToRoleAsync(staffUser, staffRole);
    }
}

app.Run();