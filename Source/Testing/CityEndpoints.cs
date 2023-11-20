using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Testing.Data;
using Testing.Data.Dtos;
using Testing.Data.Entities;
using Testing.Helpers;

namespace Testing
{
    public class CityEndpoints
    {
        public static void AddCityApi(RouteGroupBuilder CitiesGroup)
        {

            //  api/cities?pageNumber=1&pageSize=5
            CitiesGroup.MapGet("cities", async ([AsParameters] SearchParameters searchParams, AppdbContext dbContext, LinkGenerator LinkGenerator, HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                // change order by to sort by names if needed
                var queryable = dbContext.cities.AsQueryable().OrderBy(o => o.Id);
                var pagedList = await PagedList<City>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);

                var previousPageLink = pagedList.HasPrevious ? LinkGenerator.GetUriByName(httpContext, "GetCities",
                    new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                : null;
                var nextPageLink = pagedList.HasNext ? LinkGenerator.GetUriByName(httpContext, "GetCities",
                    new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                : null;

                var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize,
                    pagedList.CurrentPage, pagedList.TotalPages, previousPageLink, nextPageLink);

                httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));


                return pagedList.Select(city => new CityDto(city.Id, city.CityName, city.Country));
                //return (await dbContext.cities.ToListAsync(cancellationToken))
                //        .Select(city => new CityDto(city.Id, city.CityName, city.Country));
            }).WithName("GetCities");


            CitiesGroup.MapGet("cities/{cityId:int}", async (int cityId, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId); ;

                if (city == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new CityDto(city.Id, city.CityName, city.Country));
            }).WithName("GetCity");
            CitiesGroup.MapPost("cities/", async ([Validate] CreateCityDto createCityDto, HttpContext httpContext, LinkGenerator linkGenerator, AppdbContext dbContext) =>
            {

                var city = new City()
                {
                    CityName = createCityDto.CityName,
                    Country = createCityDto.Country,
                };
                dbContext.cities.Add(city);
                await dbContext.SaveChangesAsync();

                var links = CreateLinks(city.Id, httpContext, linkGenerator);
                var cityDto = new CityDto(city.Id, city.CityName, city.Country);
                var resource = new ResourceDto<CityDto>(cityDto, links.ToArray());

                return Results.Created($"/api/cities/{city.Id:int}", resource);
            }).WithName("CreateCity");
            CitiesGroup.MapPut("cities/{cityId:int}", async (int cityId, [Validate] UpdateCityDto CityDto, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                if (city == null)
                {
                    return Results.NotFound();
                }
                city.CityName = CityDto.CityName;
                city.Country = CityDto.Country;
                await dbcontext.SaveChangesAsync();
                return Results.Ok(new CityDto(city.Id, city.CityName, city.Country));
            }).WithName("EditCity");
            // cascad

            CitiesGroup.MapDelete("cities/{cityId:int}", async (int cityId, AppdbContext dbcontext) =>
            {
                var city = await dbcontext.cities.FirstOrDefaultAsync(c => c.Id == cityId);
                if (city == null)
                {
                    return Results.NotFound();
                }
                dbcontext.Remove(city);
                await dbcontext.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("RemoveCity");
        }
        static IEnumerable<LinkDto> CreateLinks(int cityId, HttpContext httpContext, LinkGenerator linkGenerator)
        {
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetCity", new { cityId }), "self", "GET");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditCity", new { cityId }), "edit", "PUT");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveCity", new { cityId }), "delete", "DELETE");
        }
    }



}
