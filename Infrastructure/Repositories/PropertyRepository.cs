using MongoDB.Bson;
using MongoDB.Driver;
using real_estate_api.Application.Exceptions;
using real_estate_api.Domain.Entities;

namespace real_estate_api.Infrastructure.Repositories
{
    public interface IPropertyRepository
    {
        Task<List<Property>> GetAllAsync();
        Task<Property?> GetByIdAsync(string id);
        Task<List<Property>> GetFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice);
        Task<Property> CreateAsync(Property property);
        Task<bool> UpdateAsync(string id, Property property);
        Task<bool> DeleteAsync(string id);
    }

    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly ILogger<PropertyRepository> _logger;

        public PropertyRepository(IMongoDatabase database, ILogger<PropertyRepository> logger)
        {
            _properties = database.GetCollection<Property>("Properties");
            _logger = logger;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            try
            {
                return await _properties.Find(_ => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error retrieving all properties from database");
                throw new DatabaseException("Failed to retrieve properties from database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving all properties");
                throw new DatabaseException("An unexpected error occurred while retrieving properties", ex);
            }
        }

        public async Task<Property?> GetByIdAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    throw new ValidationException("Id", "Invalid property ID format");
                }

                return await _properties.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error retrieving property {PropertyId} from database", id);
                throw new DatabaseException($"Failed to retrieve property with id {id} from database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving property {PropertyId}", id);
                throw new DatabaseException($"An unexpected error occurred while retrieving property with id {id}", ex);
            }
        }

        public async Task<List<Property>> GetFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                // Validate price range
                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    throw new ValidationException("Price", "Minimum price cannot be greater than maximum price");
                }

                var filterBuilder = Builders<Property>.Filter;
                var filters = new List<FilterDefinition<Property>>();

                filters.Add(filterBuilder.Empty);

                if (!string.IsNullOrEmpty(name))
                {
                    filters.Add(filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(name, "i")));
                }

                if (!string.IsNullOrEmpty(address))
                {
                    filters.Add(filterBuilder.Regex(p => p.Address, new MongoDB.Bson.BsonRegularExpression(address, "i")));
                }

                if (minPrice.HasValue)
                {
                    filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));
                }

                if (maxPrice.HasValue)
                {
                    filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));
                }

                var combinedFilter = filterBuilder.And(filters);
                return await _properties.Find(combinedFilter).ToListAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error filtering properties in database");
                throw new DatabaseException("Failed to filter properties from database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error filtering properties");
                throw new DatabaseException("An unexpected error occurred while filtering properties", ex);
            }
        }

        public async Task<Property> CreateAsync(Property property)
        {
            try
            {
                await _properties.InsertOneAsync(property);
                _logger.LogInformation("Property {PropertyId} created successfully", property.Id);
                return property;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error creating property in database");
                throw new DatabaseException("Failed to create property in database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating property");
                throw new DatabaseException("An unexpected error occurred while creating property", ex);
            }
        }

        public async Task<bool> UpdateAsync(string id, Property property)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    throw new ValidationException("Id", "Invalid property ID format");
                }

                var result = await _properties.ReplaceOneAsync(p => p.Id == id, property);

                if (result.MatchedCount == 0)
                {
                    throw new NotFoundException("Property", id);
                }

                _logger.LogInformation("Property {PropertyId} updated successfully", id);
                return result.ModifiedCount > 0;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error updating property {PropertyId} in database", id);
                throw new DatabaseException($"Failed to update property with id {id} in database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating property {PropertyId}", id);
                throw new DatabaseException($"An unexpected error occurred while updating property with id {id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    throw new ValidationException("Id", "Invalid property ID format");
                }

                var result = await _properties.DeleteOneAsync(p => p.Id == id);

                if (result.DeletedCount == 0)
                {
                    throw new NotFoundException("Property", id);
                }

                _logger.LogInformation("Property {PropertyId} deleted successfully", id);
                return true;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error deleting property {PropertyId} from database", id);
                throw new DatabaseException($"Failed to delete property with id {id} from database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting property {PropertyId}", id);
                throw new DatabaseException($"An unexpected error occurred while deleting property with id {id}", ex);
            }
        }
    }
}
