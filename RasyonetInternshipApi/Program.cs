using Microsoft.EntityFrameworkCore;
using RasyonetInternshipApi.Configuration;
using RasyonetInternshipApi.Data;
using RasyonetInternshipApi.Repositories;
using RasyonetInternshipApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FinnhubOptions>(
    builder.Configuration.GetSection(FinnhubOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddHttpClient<IFinancialDataService, FinnhubFinancialDataService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
