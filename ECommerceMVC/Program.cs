using ECommerceMVC.Data;
using ECommerceMVC.Data.Api;
using ECommerceMVC.Data.ErrorLogger;
using ECommerceMVC.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ECommerceConnectionString") ?? throw new InvalidOperationException("Connection string 'BlogContextConnection' not found.");

builder.Services.AddDbContext<EcommerceApiContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IClientRepo, ClientRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IErrorLogger, ErrorLoggerRepo>();
builder.Services.AddScoped<ClientApiRepo>();
builder.Services.AddScoped<ProductApiRepo>();
builder.Services.AddScoped<OrderApiRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
