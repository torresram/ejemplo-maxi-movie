using maxi_movie_mvc.Data;
using maxi_movie_mvc.Models;
using maxi_movie_mvc.Service;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Incluir DbContext
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieDbContext")));

/// <summary>
/// AddIdentityCore: Configura el sistema de autenticación y autorización de ASP.NET Core Identity
/// para la clase Usuario. Necesario para manejar registro, login y roles de usuarios.
/// </summary>
builder.Services.AddIdentityCore<Usuario>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
})
    /// <summary>
    /// AddRoles: Habilita el sistema de roles en Identity para poder asignar
    /// permisos y restricciones a diferentes grupos de usuarios.
    /// </summary>
    .AddRoles<IdentityRole>()
    /// <summary>
    /// AddEntityFrameworkStores: Indica que Identity debe usar EntityFramework Core
    /// para persistir los datos de usuarios y roles en la base de datos.
    /// </summary>
    .AddEntityFrameworkStores<MovieDbContext>()
    /// <summary>
    /// AddSignInManager: Proporciona el servicio SignInManager que maneja
    /// la lógica de login, logout y validación de credenciales de usuarios.
    /// </summary>
    .AddSignInManager();

//manejo de la cookie
/// <summary>
/// AddAuthentication: Configura el esquema de autenticación predeterminado.
/// Usa IdentityConstants.ApplicationScheme para definir cómo se autentican los usuarios.
/// </summary>
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
});

/// <summary>
/// ConfigureApplicationCookie: Personaliza el comportamiento de la cookie de autenticación.
/// - ExpireTimeSpan: Define cuánto tiempo es válida la cookie (60 minutos)
/// - SlidingExpiration: Extiende la expiración cada vez que el usuario accede a la app
/// - LoginPath: Ruta a la que redirige cuando el usuario no está autenticado
/// - AccessDeniedPath: Ruta a la que redirige cuando un usuario no tiene permisos suficientes
/// </summary>
builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    o.SlidingExpiration = true;
    o.LoginPath = "/Usuario/Login";
    o.AccessDeniedPath = "/Usuario/AccessDenied";
});

builder.Services.AddScoped<ImagenStorage>();//Registrar el servicio de almacenamiento de imágenes para inyección de dependencias
builder.Services.Configure<FormOptions>(o => { o.MultipartBoundaryLengthLimit = 2 * 1024 * 1024; }); // Limitar el tamaño máximo de los archivos subidos a 2 MB

var app = builder.Build();

//Realizar carga inical del DbSeeder con using Scope
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MovieDbContext>();
    DbSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/// <summary>
/// UseHttpsRedirection: Redirige automáticamente todas las solicitudes HTTP
/// a HTTPS para asegurar que la comunicación sea cifrada y segura.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// UseRouting: Habilita el sistema de enrutamiento que determina qué controlador
/// y acción deben ejecutarse según la URL solicitada.
/// </summary>
app.UseRouting();

app.UseAuthentication();
/// <summary>
/// UseAuthorization: Habilita el middleware de autorización que verifica
/// si el usuario autenticado tiene permisos para acceder a los recursos solicitados.
/// </summary>
app.UseAuthorization();

/// <summary>
/// MapStaticAssets: Mapea y sirve archivos estáticos (CSS, JS, imágenes, etc.)
/// desde la carpeta wwwroot. Mejora el rendimiento en producción.
/// </summary>
app.MapStaticAssets();

/// <summary>
/// MapControllerRoute: Define la ruta predeterminada para enrutar solicitudes
/// a los controladores. El patrón "{controller=Home}/{action=Index}/{id?}"
/// significa que por defecto va a Home/Index, y el {id?} es opcional.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
