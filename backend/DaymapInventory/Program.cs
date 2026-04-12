using DaymapInventory.Interfaces;
using DaymapInventory.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection - swap any In-Memory repo for a Sql implementation when the DB is ready
builder.Services.AddSingleton<IItemRepository, InMemoryItemRepository>();
builder.Services.AddSingleton<IItemInstanceRepository, InMemoryItemInstanceRepository>();
builder.Services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddSingleton<ITagRepository, InMemoryTagRepository>();
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
