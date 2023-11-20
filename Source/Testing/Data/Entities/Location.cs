using System.ComponentModel.DataAnnotations;
using Testing.Auth.model;

namespace Testing.Data.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public required Region Region { get; set; }
        [Required]
        public required string UserId { get; set; }
        public RentUser User { get; set; }
    }
}
public record LocationDto(int Id, string Name, string Description, string Address, string Picture, float Price, bool IsAvailable, int RegionId);