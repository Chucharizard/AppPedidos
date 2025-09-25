using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pedidosApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Confi Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Config cookie de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see [https://aka.ms/aspnetcore-hsts](https://aka.ms/aspnetcore-hsts).
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agg Aut antes de autorizacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Page de Identity (Login, Register)
app.MapRazorPages();

//  Crear roles automa al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Cliente", "Empleado" };

    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Crear usuario admin 
    var adminUser = await userManager.FindByEmailAsync("Admin@gmail.com");
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "Admin@gmail.com",
            Email = "Admin@gmail.com",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin123.");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    // Crear usuario Empleado
    var empleadoUser = await userManager.FindByEmailAsync("Empleado@gmail.com");
    if (empleadoUser == null)
    {
        empleadoUser = new IdentityUser
        {
            UserName = "Empleado@gmail.com",
            Email = "Empleado@gmail.com",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(empleadoUser, "Empleado123.");
        await userManager.AddToRoleAsync(empleadoUser, "Empleado");
    }
}

app.Run();
