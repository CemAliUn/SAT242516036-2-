using DbContexts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyDbModels;
using Providers;
using SAT242516036.Components;
using SAT242516036.Components.Account;
using SAT242516036.Data;
using UnitOfWorks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

//// DBCONTEXTS
// 1. Mevcut Scoped Context (BUNU SÝLME, DÝÐER SERVÝSLER ÝÇÝN LAZIM)
builder.Services.AddDbContext<MyDbModel_DbContext>(options => options.UseSqlServer(connectionString));


// 2. YENÝ EKLENEN FACTORY (BLAZOR SAYFASI ÝÇÝN BU LAZIM)
builder.Services.AddDbContextFactory<MyDbModel_DbContext>(options =>
    options.UseSqlServer(connectionString),
    lifetime: ServiceLifetime.Scoped);


//// UNITOFWORKS
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();

//// MODELS
builder.Services.AddScoped(typeof(IMyDbModel<>), typeof(MyDbModel<>));

//// PROVIDERS
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();