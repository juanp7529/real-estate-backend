using real_estate_api.Application.DTO;
using real_estate_api.Application.Exceptions;
using real_estate_api.Domain.Entities;
using real_estate_api.Infrastructure.Repositories;

namespace real_estate_api.Application.Services
{
    public interface IPropertyService
    {
        Task<List<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDetailDto?> GetPropertyByIdAsync(string id);
        Task<List<PropertyDto>> GetFilteredPropertiesAsync(PropertyFilterDto filter);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(IPropertyRepository propertyRepository, ILogger<PropertyService> logger)
        {
            _propertyRepository = propertyRepository;
            _logger = logger;
        }

        public async Task<List<PropertyDto>> GetAllPropertiesAsync()
        {
            try
            {
                var properties = await _propertyRepository.GetAllAsync();

                if (properties == null || !properties.Any())
                {
                    _logger.LogInformation("No properties found in database");
                    return new List<PropertyDto>();
                }

                return properties.Select(MapToDto).ToList();
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllPropertiesAsync service method");
                throw new BusinessException("Failed to retrieve properties", ex);
            }
        }

        public async Task<PropertyDetailDto?> GetPropertyByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ValidationException("Id", "Property ID cannot be empty");
                }

                var property = await _propertyRepository.GetByIdAsync(id);

                if (property == null)
                {
                    throw new NotFoundException("Property", id);
                }

                return MapToDetailDto(property);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPropertyByIdAsync service method for id {PropertyId}", id);
                throw new BusinessException($"Failed to retrieve property details for id {id}", ex);
            }
        }

        public async Task<List<PropertyDto>> GetFilteredPropertiesAsync(PropertyFilterDto filter)
        {
            try
            {
                if (filter == null)
                {
                    throw new ValidationException("Filter cannot be null");
                }

                // Business rule: validate price range
                if (filter.MinPrice.HasValue && filter.MinPrice.Value < 0)
                {
                    throw new ValidationException("MinPrice", "Minimum price cannot be negative");
                }

                if (filter.MaxPrice.HasValue && filter.MaxPrice.Value < 0)
                {
                    throw new ValidationException("MaxPrice", "Maximum price cannot be negative");
                }

                if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
                {
                    throw new ValidationException("Price", "Minimum price cannot be greater than maximum price");
                }

                var properties = await _propertyRepository.GetFilteredAsync(
                    filter.Name,
                    filter.Address,
                    filter.MinPrice,
                    filter.MaxPrice
                );

                if (properties == null || !properties.Any())
                {
                    _logger.LogInformation("No properties found matching the filter criteria");
                    return new List<PropertyDto>();
                }

                return properties.Select(MapToDto).ToList();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFilteredPropertiesAsync service method");
                throw new BusinessException("Failed to filter properties", ex);
            }
        }

        private PropertyDto MapToDto(Property property)
        {
            try
            {
                return new PropertyDto
                {
                    Id = property.Id,
                    IdOwner = property.Owner?.OwnerId ?? string.Empty,
                    OwnerName = property.Owner?.Name ?? string.Empty,
                    PropertyName = property.Name,
                    Address = property.Address,
                    Price = property.Price,
                    Image = property.Images?.FirstOrDefault(img => img.Enabled)?.File
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping property {PropertyId} to DTO", property.Id);
                throw new BusinessException($"Failed to map property {property.Id} to DTO", ex);
            }
        }

        private PropertyDetailDto MapToDetailDto(Property property)
        {
            try
            {
                return new PropertyDetailDto
                {
                    Id = property.Id,
                    Name = property.Name,
                    Address = property.Address,
                    Price = property.Price,
                    CodeInternal = property.CodeInternal,
                    Year = property.Year,
                    OwnerId = property.Owner?.OwnerId ?? string.Empty,
                    OwnerName = property.Owner?.Name ?? string.Empty,
                    OwnerAddress = property.Owner?.Address ?? string.Empty,
                    OwnerPhoto = property.Owner?.Photo,
                    OwnerBirthday = property.Owner?.Birthday,
                    Images = property.Images?.Select(img => new PropertyImageDto
                    {
                        Id = img.ImageId,
                        File = img.File,
                        Enabled = img.Enabled
                    }).ToList() ?? new List<PropertyImageDto>(),
                    Traces = property.Traces?.Select(trace => new PropertyTraceDto
                    {
                        Id = trace.TraceId,
                        Name = trace.Name,
                        DateSale = trace.DateSale,
                        Value = trace.Value,
                        Tax = trace.Tax
                    }).ToList() ?? new List<PropertyTraceDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping property {PropertyId} to detail DTO", property.Id);
                throw new BusinessException($"Failed to map property {property.Id} to detail DTO", ex);
            }
        }
    }
}
