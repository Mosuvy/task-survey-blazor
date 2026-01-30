using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Components;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.Repositories;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Services;
using TaskSurvey.Infrastructure.Services.IServices;
using TaskSurvey.StateServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(
//         builder.Configuration.GetConnectionString("DefaultConnection")
//     )
// );

builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? builder.Environment.WebRootPath) 
});

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("blazor_tasksurvey");

builder.Services.AddControllers();

// Add Repository
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();

// Add Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

// State Service
builder.Services.AddScoped<AuthState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    using (var scope = app.Services.CreateScope())
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();
        
        bool success = false;
        int retries = 0;

        while (!success && retries < 10)
        {
            try
            {
                Console.WriteLine($"Mencoba koneksi ke database (Percobaan {retries + 1})...");
                db.Database.EnsureDeleted();
                Console.WriteLine("Database lama berhasil di-drop.");
                db.Database.Migrate();
                Console.WriteLine("Migrasi berhasil dijalankan.");
                
                success = true;
            }
            catch (Exception ex)
            {
                retries++;
                Console.WriteLine($"Gagal: {ex.Message}. Menunggu 5 detik...");
                Thread.Sleep(5000);
            }
        }

        if (!success)
        {
            Console.WriteLine("Gagal menjalankan migrasi setelah beberapa kali percobaan. Aplikasi mungkin akan error.");
        }
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
