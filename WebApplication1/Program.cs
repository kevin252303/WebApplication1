using WebApplication1.Data;
using WebApplication1.Midddleware;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interfaces;
using WebApplication1.Services;
using WebApplication1.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Extentions;
using WebApplication1.SignalR;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Information);
});




var app = builder.Build();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<DataContext>();
try
{
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("truncate table [connections]");
    await Seed.SeedUsers(context);
    var result = context.Users.FirstOrDefault();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An Error occured during migration");
}
app.Run();
