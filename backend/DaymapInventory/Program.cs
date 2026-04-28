using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core - MySQL via Pomelo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Dependency Injection - SQL Repositories
builder.Services.AddScoped<IItemRepository, SqlItemRepository>();
builder.Services.AddScoped<IItemInstanceRepository, SqlItemInstanceRepository>();
builder.Services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
builder.Services.AddScoped<ITagRepository, SqlTagRepository>();
builder.Services.AddScoped<ITransactionRepository, SqlTransactionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
