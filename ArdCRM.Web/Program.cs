using ArdCRM.Business.Services;
using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;
using ArdCRM.Data.Context;
using ArdCRM.Data.Repositories;
using ArdCRM.Data.Adapters;
using ArdCRM.Core.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// appsettings.{Environment}.json yükle
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// DataProtection — key'leri proje klasöründe tut
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));

// DbContext
builder.Services.AddDbContext<ArdCrmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository kayıtları
builder.Services.AddScoped<IRepository<Musteri>, GenericRepository<Musteri>>();
builder.Services.AddScoped<IRepository<Teklif>, GenericRepository<Teklif>>();

// Servis kayıtları
builder.Services.AddScoped<IMusteriService, MusteriService>();
builder.Services.AddScoped<ITeklifService, TeklifService>();
builder.Services.AddScoped<IErpSenkronService, ErpSenkronService>();

// ERP Adapter — NetsisConnection tanımlıysa aktif, yoksa NullAdapter
var netsisConn = builder.Configuration.GetConnectionString("NetsisConnection");
if (!string.IsNullOrEmpty(netsisConn))
    builder.Services.AddScoped<IErpAdapter>(_ => new NetsisAdapter(netsisConn));
else
    builder.Services.AddScoped<IErpAdapter, NullErpAdapter>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
