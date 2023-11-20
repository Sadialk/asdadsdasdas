using System.ComponentModel.DataAnnotations;
using Testing.Auth.model;

namespace Testing.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public required string CityName { get; set; }
        public required string Country { get; set; }

    }
}
public record CityDto(int Id, string CityName, string Country);