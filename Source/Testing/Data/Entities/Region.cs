using System.ComponentModel.DataAnnotations;
using Testing.Auth.model;
using Testing.Data.Entities;

namespace Testing.Data.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public required City City { get; set; }

    }
}
public record RegionDto(int Id, string Name, int CityId);