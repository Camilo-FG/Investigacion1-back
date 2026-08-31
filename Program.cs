using Investigacion1_back.Features.Auth.AdminRegister;
using Investigacion1_back.Features.Auth.Login;
using Investigacion1_back.Features.Auth.Logout;
using Investigacion1_back.Features.Auth.Refresh;
using Investigacion1_back.Features.Auth.Register;
using Investigacion1_back.Features.Reservations.CreateReservation;
using Investigacion1_back.Features.Reservations.GetReservations;
using Investigacion1_back.Features.Rooms.CreateRoom;
using Investigacion1_back.Features.Rooms.GetRooms;
using Investigacion1_back.Features.Users.GetMe;
using Investigacion1_back.Features.Users.GetUserById;
using Investigacion1_back.Features.Users.GetUsers;
using Investigacion1_back.Features.Users.UpdateSubscriptionExpiration;
using Investigacion1_back.Features.Users.UpdateUserStatus;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (!string.IsNullOrWhiteSpace(jwtSecret))
{
    builder.Configuration["Jwt:Secret"] = jwtSecret;
}

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddJwtAccessTokenValidation(builder.Configuration);
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<AdminRegisterHandler>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<GetMeHandler>();
builder.Services.AddScoped<GetUsersHandler>();
builder.Services.AddScoped<GetUserByIdHandler>();
builder.Services.AddScoped<UpdateUserStatusHandler>();
builder.Services.AddScoped<UpdateSubscriptionExpirationHandler>();
builder.Services.AddScoped<CreateReservationHandler>();
builder.Services.AddScoped<GetReservationsHandler>();
builder.Services.AddScoped<CreateRoomHandler>();
builder.Services.AddScoped<GetRoomsHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(ResolveConnectionString(builder.Configuration)));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JSON serialization for minimal APIs
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",
                  "http://localhost:5174",
                  "http://localhost:5175")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseSubscriptionExpirationCheck();
app.UseAuthorization();

app.MapControllers();
LoginEndpoint.Map(app);
RegisterEndpoint.Map(app);
AdminRegisterEndpoint.Map(app);
RefreshEndpoint.Map(app);
LogoutEndpoint.Map(app);
GetMeEndpoint.Map(app);
GetUsersEndpoint.Map(app);
GetUserByIdEndpoint.Map(app);
UpdateUserStatusEndpoint.Map(app);
UpdateSubscriptionExpirationEndpoint.Map(app);
CreateReservationEndpoint.Map(app);
GetReservationsEndpoint.Map(app);
CreateRoomEndpoint.Map(app);
GetRoomsEndpoint.Map(app);

// Seed admin user if none exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwords = scope.ServiceProvider.GetRequiredService<PasswordService>();

    if (!db.Users.Any(u => u.Role == Roles.Admin))
    {
        var adminEmail = "admin@example.com";
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = adminEmail,
            PasswordHash = passwords.Hash("AdminPass1"),
            Role = Roles.Admin,
            IsActive = true,
            SubscriptionExpirationDate = DateTime.UtcNow.AddYears(100)
        };
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        Console.WriteLine($"Admin user seeded: {adminEmail}");
    }
}

app.Run();

static string ResolveConnectionString(IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    if (string.IsNullOrWhiteSpace(password))
        throw new InvalidOperationException("POSTGRES_PASSWORD is not set. Add it to your .env file.");

    return connectionString.Replace("[YOUR-PASSWORD]", password, StringComparison.Ordinal);
}
