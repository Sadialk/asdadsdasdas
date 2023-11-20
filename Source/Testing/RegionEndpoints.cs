using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using Testing.Data;
using Testing.Data.Dtos;
using Testing.Data.Entities;

namespace Testing
{
    public class RegionEndpoints
    {
        public static void AddRegionApi(RouteGroupBuilder RegionGroup)
        {
            RegionGroup.MapGet("/regions", async (int cityId, AppdbContext dbContext, CancellationToken cancellationToken) =>
            {
                return (await dbContext.regions.Include(city => city.City).ToListAsync(cancellationToken))
                        .Where(region => region.City.Id == cityId)
                        .Select(region => new RegionDto(region.Id, region.Name, region.City.Id));
            });
            RegionGroup.MapGet("regions/{regionId:int}", async (int cityId, int regionId, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                if (city == null || region == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new RegionDto(region.Id, region.Name, cityId));
            });
            RegionGroup.MapPost("regions/", async (int cityId, [Validate] CreateRegionDto createRegionDto, HttpContext httpContext, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId); ;
                if (city == null)
                {
                    return Results.NotFound();
                }
                var region = new Region()
                {
                    Name = createRegionDto.Name,
                    City = city,
                };
                dbcontext.regions.Add(region);
                await dbcontext.SaveChangesAsync();

                return Results.Created($"/api/cities/{createRegionDto.CityId:int}",
                                        new RegionDto(region.Id, region.Name, region.City.Id));
            });
            RegionGroup.MapPut("regions/{regionId:int}", async (int cityId, int regionId, [Validate] UpdateRegionDto regionDto, AppdbContext dbcontext) =>
            {
                Console.WriteLine(cityId + "   " + regionId);
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.Include(city => city.City).FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                if (city == null || region == null)
                {
                    return Results.NotFound();
                }
                region.Name = regionDto.Name;
                await dbcontext.SaveChangesAsync();
                return Results.Ok(new RegionDto(region.Id, region.Name, cityId));
            });
            RegionGroup.MapDelete("regions/{regionId:int}", async (int cityId, int regionId, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                var region = await dbcontext.regions.Include(city => city.City).FirstOrDefaultAsync(r => r.Id == regionId && r.City.Id == cityId);
                if (city == null || region == null)
                {
                    return Results.NotFound();
                }
                dbcontext.Remove(region);
                await dbcontext.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
