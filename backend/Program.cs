using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
                        };
                });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseTrackerDbContext>();
    await DbInitializer.InitializeAsync(dbContext);
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();