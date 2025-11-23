using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProperlyASPPages.Data;
using Properly.Models;
using ProperlyASPPages.Services;
using ProperlyASPPages.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProperlyASPPages.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ProperlyIdentityDB") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ProperlyIdentityDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => 
    {
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    }));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ProperlyIdentityDbContext>();

// Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ISystemAdminRepository, SystemAdminRepository>();

// Services
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Authorization
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, SystemAdminHandler>();
builder.Services.AddAuthorization(options =>
{
    SystemAdminPolicies.AddSystemAdminPolicies(options);
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
