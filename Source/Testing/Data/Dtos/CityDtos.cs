using FluentValidation;

namespace Testing.Data.Dtos
{
    public record CreateCityDto(string CityName, string Country);
    public record UpdateCityDto(string CityName, string Country);

    public record CreateRegionDto(string Name, int CityId);
    public record UpdateRegionDto(string Name, int CityId);

    public record CreateLocationDto(int Id, string Name, string Description, string Address, string Picture, float Price, bool IsAvailable, int RegionId);
    public record UpdateLocationDto(int Id, string Name, string Description, string Address, string Picture, float Price, bool IsAvailable, int RegionId);
    public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
    {
        public CreateCityDtoValidator()
        {
            RuleFor(dto => dto.CityName).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Country).NotEmpty().NotNull().Length(2, 50);
        }
    }
    public class UpdateCityDtoValidator : AbstractValidator<UpdateCityDto>
    {
        public UpdateCityDtoValidator()
        {
            RuleFor(dto => dto.CityName).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Country).NotEmpty().NotNull().Length(2, 50);
        }
    }
    public class CreateRegionDtoValidator : AbstractValidator<CreateRegionDto>
    {
        public CreateRegionDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(2, 50);
        }
    }
    public class UpdateRegionDtoValidator : AbstractValidator<UpdateRegionDto>
    {
        public UpdateRegionDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(2, 50);
        }
    }

    public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Address).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Picture).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Price).NotEmpty().NotNull();
        }
    }
    public class UpdateLocationDtoValidator : AbstractValidator<CreateLocationDto>
    {
        public UpdateLocationDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Address).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Picture).NotEmpty().NotNull().Length(2, 50);
            RuleFor(dto => dto.Price).NotEmpty().NotNull();
        }
    }


}
