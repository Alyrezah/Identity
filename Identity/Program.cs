using FluentValidation;
using Identity.Core.Application.ClaimsStore;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.Contracts.Infrastructure;
using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Application.DTOs.ProductCategory.Validators;
using Identity.Core.Application.Services;
using Identity.Infrastructure;
using Identity.Infrastructure.Idnetity;
using Identity.Infrastructure.Idnetity.Models;
using Identity.Infrastructure.Idnetity.Services;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repository;
using Identity.Security.Default;
using Identity.Security.DynamicRole;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddDataProtection();

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
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ISiteSettingService, SiteSettingService>();

builder.Services.AddTransient<IMessageSender, MessageSender>();
builder.Services.AddTransient<IUtilities, Utilities>();
builder.Services.AddTransient<IPhoneTotpProvider, PhoneTotpProvider>();
builder.Services.Configure<PhoneTotpOptions>(o =>
{
    o.StepInSeconds = 60;
});

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "IdentityProj";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

builder.Services.Configure<SecurityStampValidatorOptions>(optins =>{
    optins.ValidationInterval = TimeSpan.FromSeconds(5);
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "491550122729-q1tjfmiidn0493bnllp6j2upqd8uoums.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductCategoriesList", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.ProductCategoriesList, allowedValues: true.ToString());
    });
    options.AddPolicy("CreateProductCategory", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.CreateProductCategory, allowedValues: true.ToString());
    });
    options.AddPolicy("EditProductCategory", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.EditProductCategory, allowedValues: true.ToString());
    });
    options.AddPolicy("DetailProductCategory", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.DetailProductCategory, allowedValues: true.ToString());
    });
    options.AddPolicy("ProductsList", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.ProductsList, allowedValues: true.ToString());
    });
    options.AddPolicy("CreateProduct", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.CreateProduct, allowedValues: true.ToString());
    });
    options.AddPolicy("EditProduct", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.EditProduct, allowedValues: true.ToString());
    });
    options.AddPolicy("DetailProduct", p =>
    {
        p.RequireClaim(claimType: ClaimTypesStore.DetailProduct, allowedValues: true.ToString());
    });
    options.AddPolicy("AccountsList", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
            c.User.HasClaim(ClaimTypesStore.AccountsList, true.ToString()));
    });
    options.AddPolicy("DetailAccount", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
           c.User.HasClaim(ClaimTypesStore.DetailAccount, true.ToString()));
    });
    options.AddPolicy("ManageUserRole", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
          c.User.HasClaim(ClaimTypesStore.ManageUserRole, true.ToString()));
    });
    options.AddPolicy("Roles", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
          c.User.HasClaim(ClaimTypesStore.Roles, true.ToString()));
    });
    options.AddPolicy("AddClaims", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
        c.User.HasClaim(ClaimTypesStore.AddClaims, true.ToString()));
    });
    options.AddPolicy("RemoveClaims", p =>
    {
        p.RequireAssertion(c => c.User.IsInRole("Owner") ||
    c.User.HasClaim(ClaimTypesStore.RemoveClaims, true.ToString()));
    });
    options.AddPolicy("ClaimRequirement", p =>
    {
        p.Requirements.Add(new ClaimRequirement(ClaimTypesStore.AdministratorPanel, true.ToString()));
    });
    options.AddPolicy("DynamicRole", p =>
    {
        p.Requirements.Add(new DynamicRoleRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, ClaimRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DynamicRoleRequirementHandler>();

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
