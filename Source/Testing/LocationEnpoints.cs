using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;
using System.Data;
using System.Net.Http;
using System.Security.Claims;
using Testing.Auth.model;
using Testing.Data;
using Testing.Data.Dtos;
using Testing.Data.Entities;

namespace Testing
{
    public class LocationEnpoints
    {
        public static void AddLocationApi(RouteGroupBuilder LocationGroup)
        {
            LocationGroup.MapGet("/locations", async (int regionId, AppdbContext dbContext, CancellationToken cancellationToken) =>
            {

                return (await dbContext.locations.Include(location => location.Region).ToListAsync(cancellationToken))
                        .Where(location => location.Region.Id == regionId)
                        .Select(location => new LocationDto(location.Id, location.Name, location.Description, location.Address, location.Picture, location.Price, location.IsAvailable, location.Region.Id));
            });
            LocationGroup.MapGet("locations/{locationId:int}", [Authorize(Roles = RentRoles.RentUser)] async (int cityId, int regionId, int locationId, AppdbContext dbcontext, HttpContext httpContext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                var location = await dbcontext.locations.FirstOrDefaultAsync(l => l.Id == locationId && l.Region.Id == regionId && l.Region.City.Id == cityId);

                if (city == null || region == null || location == null)
                {
                    return Results.NotFound();
                }
                if(!httpContext.User.IsInRole(RentRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != location.UserId)
                {
                    return Results.Forbid();
                }

                return Results.Ok(new LocationDto(location.Id, location.Name, location.Description, location.Address, location.Picture, location.Price, location.IsAvailable, location.Id));
            });
            LocationGroup.MapPost("locations/", async (int regionId, [Validate] CreateLocationDto createLocationDto, HttpContext httpContext, AppdbContext dbcontext) =>
            {
                var region = await dbcontext.regions.Include(region => region.City).FirstOrDefaultAsync(c => c.Id == regionId);
                if (region == null)
                {
                    return Results.NotFound();
                }
                var location = new Location()
                {
                    Name = createLocationDto.Name,
                    Description = createLocationDto.Description,
                    Address = createLocationDto.Address,
                    Picture = createLocationDto.Picture,
                    Price = createLocationDto.Price,
                    IsAvailable = createLocationDto.IsAvailable,
                    Region = region,
                    UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                };
                dbcontext.locations.Add(location);
                await dbcontext.SaveChangesAsync();

                return Results.Created($"/api/cities/{createLocationDto.Id:int}",
                                        new LocationDto(location.Id, location.Name, location.Description, location.Address, location.Picture, location.Price, location.IsAvailable, location.Id));
            });
            LocationGroup.MapPut("locations/{locationId:int}", async (int cityId, int regionId, int locationId, HttpContext httpContext, [Validate] CreateLocationDto createLocationDto, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                var location = await dbcontext.locations.FirstOrDefaultAsync(l => l.Id == locationId && l.Region.Id == regionId && l.Region.City.Id == cityId);
                Console.WriteLine(createLocationDto.IsAvailable);
                if (city == null || region == null || location == null)
                {
                    return Results.NotFound();
                }
                location.Name = createLocationDto.Name;
                location.Description = createLocationDto.Description;
                location.Address = createLocationDto.Address;
                location.Picture = createLocationDto.Picture;
                location.Price = createLocationDto.Price;
                location.IsAvailable = createLocationDto.IsAvailable;


                await dbcontext.SaveChangesAsync();
                return Results.Ok(new LocationDto(location.Id, location.Name, location.Description, location.Address, location.Picture, location.Price, location.IsAvailable, regionId));
            });
            LocationGroup.MapDelete("locations/{locationId:int}", [Authorize(Roles = RentRoles.RentUser)] async (int cityId, int regionId, int locationId, HttpContext httpContext, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                var location = await dbcontext.locations.FirstOrDefaultAsync(l => l.Id == locationId && l.Region.Id == regionId && l.Region.City.Id == cityId);
                if (city == null || region == null || location == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(RentRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != location.UserId)
                {
                    //not found to hide it
                    return Results.Forbid();
                }
               
                dbcontext.Remove(location);
                await dbcontext.SaveChangesAsync();
                return Results.NoContent();
            });

        }
    }
}
