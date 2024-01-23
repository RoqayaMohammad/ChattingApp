using ChattingApp.Data;
using ChattingApp.Extensions;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using ChattingApp.Sevices;
using ChattingApp.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();

//if (builder.Environment.IsDevelopment()){
//    app.UseDeveloperExceptionPage();
//}

app.UseCors(builder=> builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5118")); //front

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<PresenceHub>("Hubs/presence");
app.MapHub<MessageHub>("Hubs/message");


using var scope=app.Services.CreateScope();
var services=scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager=services.GetRequiredService<UserManager<AppUser>>();
    var roleManager= services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Connections]");
    await Seed.SeedUsers(userManager,roleManager);
}
catch(Exception ex)
{
    var logger= services.GetService<ILogger<Program>>();
    logger.LogError(ex, "Error occured whilr migrating Process");
}
app.Run();
