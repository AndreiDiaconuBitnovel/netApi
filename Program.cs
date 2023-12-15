using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DataDB;
using WebApplication2.Services.TestDbServiceFolder;
using WebApplication2.Services.TranslateLanguageFolder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TeamDropDatabaseContext>(options =>
{
    options.UseSqlServer("Name=ConnectionStrings");
});

builder.Services.AddCors(options => options.AddPolicy("FrontEnd", policy =>
{
    policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddScoped<ITestDBService, TestDBService>();
builder.Services.AddScoped<ITranslateLanguageService, TranslateLanguageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontEnd");
app.UseAuthorization();

app.MapControllers();

app.Run();
