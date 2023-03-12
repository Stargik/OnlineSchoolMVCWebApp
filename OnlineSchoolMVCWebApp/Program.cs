using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;
using OnlineSchoolMVCWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ExcelService>();

builder.Services.AddDbContext<OnlineSchoolDbContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString(SettingStrings.OnlineSchoolDbConnection)
));

builder.Services.AddDbContext<IdentityOnlineSchoolDbContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString(SettingStrings.IdentityOnlineSchoolDbConnection)
    ));

builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<IdentityOnlineSchoolDbContext>();


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

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
