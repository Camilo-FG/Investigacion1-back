using Investigacion1_back.Features.Auth.AdminRegister;
using Investigacion1_back.Features.Auth.Login;
using Investigacion1_back.Features.Auth.Logout;
using Investigacion1_back.Features.Auth.Refresh;
using Investigacion1_back.Features.Auth.Register;
using Investigacion1_back.Features.Users.GetMe;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddJwtAccessTokenValidation(builder.Configuration);
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<AdminRegisterHandler>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<GetMeHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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

app.Run();
