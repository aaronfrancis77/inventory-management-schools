using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Repositories;
using DaymapInventory.Helpers;  
using DaymapInventory.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{   options.Filters.AddService<LocalDateTimeFilter>();});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core - SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - SQL Repositories
builder.Services.AddScoped<IItemRepository, SqlItemRepository>();
builder.Services.AddScoped<IItemInstanceRepository, SqlItemInstanceRepository>();
builder.Services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
builder.Services.AddScoped<ITagRepository, SqlTagRepository>();
builder.Services.AddScoped<ITransactionRepository, SqlTransactionRepository>();

// UTC Time to Local Time 
// Register DateTimeHelper and filter
builder.Services.AddSingleton<DateTimeHelper>();
builder.Services.AddScoped<LocalDateTimeFilter>();


var app = builder.Build();

// Auto-apply EF migrations on startup (useful in Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
