using FluentValidation;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.Contracts.Infrastructure;
using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Application.DTOs.ProductCategory.Validators;
using Identity.Core.Application.Services;
using Identity.Infrastructure;
using Identity.Infrastructure.Idnetity;
using Identity.Infrastructure.Idnetity.Services;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

#region Db Contexts

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString"));
});

builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString"));
});

#endregion

#region Ioc

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddTransient<IMessageSender, MessageSender>();

#endregion

#region Identity

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);
}).AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();
/*.AddErrorDescriber<PersianIdentityErrorDescriber>();*/

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "491550122729-q1tjfmiidn0493bnllp6j2upqd8uoums.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

#endregion

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<ProductCategoryValidatorBase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
