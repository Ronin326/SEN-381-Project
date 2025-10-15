using Microsoft.EntityFrameworkCore;
using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Services;
using DotNetEnv;

Env.Load(); // Load .env file if present

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Add services to the container
// ==============================

// Razor Pages & Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // API controller support

// HttpClient for ChatbotService
builder.Services.AddHttpClient<ChatbotService>();

// Custom chatbot content filter
builder.Services.AddScoped<ContentFilterService>();

// ==============================
// Database & EF Core
// ==============================
builder.Services.AddDbContext<CampusLearnDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==============================
// Custom application services
// ==============================
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ==============================
// Session configuration
// ==============================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ==============================
// Database seeding
// ==============================
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}

// ==============================
// Configure HTTP pipeline
// ==============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Default HSTS value = 30 days
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();      // Session middleware
app.UseAuthorization(); // Auth middleware

// Map routes
app.MapRazorPages();
app.MapControllers(); // Enable API routes

// ==============================
// Default route (adjust as needed)
// ==============================
app.MapGet("/", context =>
{
    // Choose your default landing page:
    // context.Response.Redirect("/LoginPage");
    // context.Response.Redirect("/Student/StudentDashboard");
    // context.Response.Redirect("/Tutor/TutorDashboard");
    context.Response.Redirect("/TestChat"); // Chatbot test page
    return Task.CompletedTask;
});

app.Run();
