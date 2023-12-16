using Testing;
using Microsoft.EntityFrameworkCore;
using Testing.Data.Entities;
using FluentValidation;
using O9d.AspNet.FluentValidation;
using System.Runtime.CompilerServices;
using System.Net;
using Testing.Data.Dtos;
using Testing.Data;
using Microsoft.AspNetCore.Identity;
using Testing.Auth.model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Testing.Auth;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//  https://monkfish-app-zesk6.ondigitalocean.app/
var origin = "tavomama";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DigitalOceanDBConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: origin,
                      policy =>
                      {
                          policy.WithOrigins("https://monkfish-app-zesk6.ondigitalocean.app/",
                              "*");
                      });
});

builder.Services.AddIdentity<RentUser, IdentityRole>()
    .AddEntityFrameworkStores<AppdbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddAuthorization();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


var CitiesGroup = app.MapGroup("/api").WithValidationFilter();
CityEndpoints.AddCityApi(CitiesGroup);
var RegionGroup = app.MapGroup("/api/cities/{cityId}").WithValidationFilter();
RegionEndpoints.AddRegionApi(RegionGroup);
var LocationGroup = app.MapGroup("/api/cities/{cityId}/regions/{regionId}").WithValidationFilter();
LocationEnpoints.AddLocationApi(LocationGroup);

app.AddAuthApi();


app.UseCors(origin);


app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<AppdbContext>();
    //dataContext.Database.Migrate();
    var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
    await dbSeeder.SeedAsync();
}
app.Run();

