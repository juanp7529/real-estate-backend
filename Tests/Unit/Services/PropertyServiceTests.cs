using FluentAssertions;
using Moq;
using NUnit.Framework;
using real_estate_api.Application.DTO;
using real_estate_api.Application.Exceptions;
using real_estate_api.Application.Services;
using real_estate_api.Domain.Entities;
using real_estate_api.Infrastructure.Repositories;

namespace real_estate_api.Tests.Unit.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _mockRepository;
        private Mock<ILogger<PropertyService>> _mockLogger;
        private PropertyService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IPropertyRepository>();
            _mockLogger = new Mock<ILogger<PropertyService>>();
            _service = new PropertyService(_mockRepository.Object, _mockLogger.Object);
        }

        #region GetAllPropertiesAsync Tests

        [Test]
        public async Task GetAllPropertiesAsync_WhenPropertiesExist_ReturnsPropertyDtoList()
        {
            // Arrange
            var properties = new List<Property>
            {
            new Property
              {
                Id = "507f1f77bcf86cd799439011",
                Name = "Casa Moderna",
                Address = "Calle 123",
                Price = 250000m,
                CodeInternal = "PROP-001",
                Year = 2020,
                Owner = new Owner
                {
                    OwnerId = "owner001",
                    Name = "Juan Pérez",
                    Address = "Calle Owner",
                    Photo = "photo.jpg",
                    Birthday = DateTime.Parse("1980-01-01")
                },
                Images = new List<PropertyImage>
                {
                    new PropertyImage { ImageId = "img001", File = "image1.jpg", Enabled = true }
                },
                Traces = new List<PropertyTrace>()
              }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                  .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].PropertyName.Should().Be("Casa Moderna");
            result[0].IdOwner.Should().Be("owner001");
            result[0].OwnerName.Should().Be("Juan Pérez");
            result[0].Address.Should().Be("Calle 123");
            result[0].Price.Should().Be(250000m);
            result[0].Image.Should().Be("image1.jpg");
        }

        [Test]
        public async Task GetAllPropertiesAsync_WhenNoProperties_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
    .ReturnsAsync(new List<Property>());

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllPropertiesAsync_WhenPropertyHasNoOwner_ReturnsEmptyOwnerFields()
        {
            // Arrange
            var properties = new List<Property>
            {
     new Property
       {
   Id = "507f1f77bcf86cd799439011",
            Name = "Casa Sin Dueño",
    Address = "Calle 456",
           Price = 150000m,
       CodeInternal = "PROP-002",
       Year = 2019,
                  Owner = null,
    Images = new List<PropertyImage>(),
           Traces = new List<PropertyTrace>()
         }
      };

            _mockRepository.Setup(r => r.GetAllAsync())
         .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            result.Should().NotBeNull();
            result[0].IdOwner.Should().BeEmpty();
            result[0].OwnerName.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllPropertiesAsync_WhenPropertyHasNoEnabledImages_ReturnsNullImage()
        {
            // Arrange
            var properties = new List<Property>
          {
                new Property
    {
  Id = "507f1f77bcf86cd799439011",
                    Name = "Casa Sin Imágenes",
             Address = "Calle 789",
          Price = 180000m,
     CodeInternal = "PROP-003",
    Year = 2021,
     Owner = new Owner
             {
OwnerId = "owner002",
    Name = "María García",
         Address = "Calle Owner 2",
       Photo = "photo2.jpg",
     Birthday = DateTime.Parse("1985-05-15")
                },
          Images = new List<PropertyImage>
     {
  new PropertyImage { ImageId = "img002", File = "image2.jpg", Enabled = false }
     },
            Traces = new List<PropertyTrace>()
         }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                  .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            result.Should().NotBeNull();
            result[0].Image.Should().BeNull();
        }

        [Test]
        public void GetAllPropertiesAsync_WhenDatabaseExceptionOccurs_ThrowsBusinessException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                     .ThrowsAsync(new DatabaseException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetAllPropertiesAsync();

            // Assert
            act.Should().ThrowAsync<DatabaseException>();
        }

        #endregion

        #region GetPropertyByIdAsync Tests

        [Test]
        public async Task GetPropertyByIdAsync_WithValidId_ReturnsPropertyDetailDto()
        {
            // Arrange
            var propertyId = "507f1f77bcf86cd799439011";
            var property = new Property
            {
                Id = propertyId,
                Name = "Casa Detallada",
                Address = "Avenida Principal 100",
                Price = 350000m,
                CodeInternal = "PROP-100",
                Year = 2022,
                Owner = new Owner
                {
                    OwnerId = "owner100",
                    Name = "Carlos López",
                    Address = "Calle del Dueño 50",
                    Photo = "carlos.jpg",
                    Birthday = DateTime.Parse("1975-03-20")
                },
                Images = new List<PropertyImage>
            {
     new PropertyImage { ImageId = "img100", File = "main.jpg", Enabled = true },
    new PropertyImage { ImageId = "img101", File = "secondary.jpg", Enabled = false }
             },
                Traces = new List<PropertyTrace>
         {
           new PropertyTrace
   {
                   TraceId = "trace100",
        Name = "Venta Inicial",
  DateSale = DateTime.Parse("2022-01-15"),
         Value = 350000m,
                 Tax = 17500m
                }
  }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId))
               .ReturnsAsync(property);

            // Act
            var result = await _service.GetPropertyByIdAsync(propertyId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(propertyId);
            result.Name.Should().Be("Casa Detallada");
            result.Address.Should().Be("Avenida Principal 100");
            result.Price.Should().Be(350000m);
            result.CodeInternal.Should().Be("PROP-100");
            result.Year.Should().Be(2022);
            result.OwnerId.Should().Be("owner100");
            result.OwnerName.Should().Be("Carlos López");
            result.OwnerAddress.Should().Be("Calle del Dueño 50");
            result.OwnerPhoto.Should().Be("carlos.jpg");
            result.Images.Should().HaveCount(2);
            result.Traces.Should().HaveCount(1);
        }

        [Test]
        public void GetPropertyByIdAsync_WithEmptyId_ThrowsValidationException()
        {
            // Arrange
            var emptyId = "";

            // Act
            Func<Task> act = async () => await _service.GetPropertyByIdAsync(emptyId);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
                 .WithMessage("*Property ID cannot be empty*");
        }

        [Test]
        public void GetPropertyByIdAsync_WithWhitespaceId_ThrowsValidationException()
        {
            // Arrange
            var whitespaceId = "   ";

            // Act
            Func<Task> act = async () => await _service.GetPropertyByIdAsync(whitespaceId);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
        .WithMessage("*Property ID cannot be empty*");
        }

        [Test]
        public void GetPropertyByIdAsync_WhenPropertyNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var propertyId = "507f1f77bcf86cd799439011";
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId))
                  .ReturnsAsync((Property?)null);

            // Act
            Func<Task> act = async () => await _service.GetPropertyByIdAsync(propertyId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
          .WithMessage($"*{propertyId}*");
        }

        [Test]
        public void GetPropertyByIdAsync_WhenDatabaseExceptionOccurs_ThrowsDatabaseException()
        {
            // Arrange
            var propertyId = "507f1f77bcf86cd799439011";
            _mockRepository.Setup(r => r.GetByIdAsync(propertyId))
          .ThrowsAsync(new DatabaseException("Connection timeout"));

            // Act
            Func<Task> act = async () => await _service.GetPropertyByIdAsync(propertyId);

            // Assert
            act.Should().ThrowAsync<DatabaseException>();
        }

        [Test]
        public async Task GetPropertyByIdAsync_WhenPropertyHasNoOwner_ReturnsEmptyOwnerFields()
        {
            // Arrange
            var propertyId = "507f1f77bcf86cd799439011";
            var property = new Property
            {
                Id = propertyId,
                Name = "Casa Sin Dueño",
                Address = "Calle Sin Dueño 1",
                Price = 200000m,
                CodeInternal = "PROP-200",
                Year = 2020,
                Owner = null,
                Images = new List<PropertyImage>(),
                Traces = new List<PropertyTrace>()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId))
       .ReturnsAsync(property);

            // Act
            var result = await _service.GetPropertyByIdAsync(propertyId);

            // Assert
            result.Should().NotBeNull();
            result!.OwnerId.Should().BeEmpty();
            result.OwnerName.Should().BeEmpty();
            result.OwnerAddress.Should().BeEmpty();
            result.OwnerPhoto.Should().BeNull();
            result.OwnerBirthday.Should().BeNull();
        }

        [Test]
        public async Task GetPropertyByIdAsync_WhenPropertyHasNoImagesOrTraces_ReturnsEmptyCollections()
        {
            // Arrange
            var propertyId = "507f1f77bcf86cd799439011";
            var property = new Property
            {
                Id = propertyId,
                Name = "Casa Básica",
                Address = "Calle Básica 1",
                Price = 150000m,
                CodeInternal = "PROP-300",
                Year = 2018,
                Owner = new Owner
                {
                    OwnerId = "owner300",
                    Name = "Ana Torres",
                    Address = "Calle Torres",
                    Photo = "ana.jpg",
                    Birthday = DateTime.Parse("1990-07-10")
                },
                Images = null,
                Traces = null
            };

            _mockRepository.Setup(r => r.GetByIdAsync(propertyId))
         .ReturnsAsync(property);

            // Act
            var result = await _service.GetPropertyByIdAsync(propertyId);

            // Assert
            result.Should().NotBeNull();
            result!.Images.Should().NotBeNull().And.BeEmpty();
            result.Traces.Should().NotBeNull().And.BeEmpty();
        }

        #endregion

        #region GetFilteredPropertiesAsync Tests

        [Test]
        public async Task GetFilteredPropertiesAsync_WithValidFilter_ReturnsFilteredProperties()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                Name = "Casa",
                Address = "Calle",
                MinPrice = 100000m,
                MaxPrice = 500000m
            };

            var properties = new List<Property>
          {
        new Property
            {
             Id = "507f1f77bcf86cd799439011",
              Name = "Casa Filtrada",
               Address = "Calle Filtro 1",
         Price = 300000m,
                CodeInternal = "PROP-400",
     Year = 2021,
         Owner = new Owner
     {
    OwnerId = "owner400",
    Name = "Pedro Martínez",
          Address = "Calle Pedro",
Photo = "pedro.jpg",
  Birthday = DateTime.Parse("1982-11-05")
     },
  Images = new List<PropertyImage>
              {
 new PropertyImage { ImageId = "img400", File = "filtered.jpg", Enabled = true }
   },
         Traces = new List<PropertyTrace>()
         }
  };

            _mockRepository.Setup(r => r.GetFilteredAsync(
  filter.Name,
    filter.Address,
     filter.MinPrice,
     filter.MaxPrice))
            .ReturnsAsync(properties);

            // Act
            var result = await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].PropertyName.Should().Be("Casa Filtrada");
            result[0].Address.Should().Contain("Calle");
            result[0].Price.Should().BeInRange(100000m, 500000m);
        }

        [Test]
        public void GetFilteredPropertiesAsync_WithNullFilter_ThrowsValidationException()
        {
            // Arrange
            PropertyFilterDto? filter = null;

            // Act
            Func<Task> act = async () => await _service.GetFilteredPropertiesAsync(filter!);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
    .WithMessage("*Filter cannot be null*");
        }

        [Test]
        public void GetFilteredPropertiesAsync_WithNegativeMinPrice_ThrowsValidationException()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                MinPrice = -1000m
            };

            // Act
            Func<Task> act = async () => await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
   .WithMessage("*Minimum price cannot be negative*");
        }

        [Test]
        public void GetFilteredPropertiesAsync_WithNegativeMaxPrice_ThrowsValidationException()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                MaxPrice = -500m
            };

            // Act
            Func<Task> act = async () => await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
    .WithMessage("*Maximum price cannot be negative*");
        }

        [Test]
        public void GetFilteredPropertiesAsync_WithMinPriceGreaterThanMaxPrice_ThrowsValidationException()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                MinPrice = 500000m,
                MaxPrice = 100000m
            };

            // Act
            Func<Task> act = async () => await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
        .WithMessage("*Minimum price cannot be greater than maximum price*");
        }

        [Test]
        public async Task GetFilteredPropertiesAsync_WhenNoPropertiesMatch_ReturnsEmptyList()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                Name = "NoExiste",
                MinPrice = 999999999m
            };

            _mockRepository.Setup(r => r.GetFilteredAsync(
             filter.Name,
              filter.Address,
                filter.MinPrice,
                      filter.MaxPrice))
                        .ReturnsAsync(new List<Property>());

            // Act
            var result = await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetFilteredPropertiesAsync_WithOnlyNameFilter_ReturnsMatchingProperties()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                Name = "Apartamento"
            };

            var properties = new List<Property>
     {
      new Property
  {
           Id = "507f1f77bcf86cd799439012",
         Name = "Apartamento Moderno",
      Address = "Torre 1",
          Price = 180000m,
    CodeInternal = "PROP-500",
              Year = 2023,
   Owner = new Owner
          {
        OwnerId = "owner500",
        Name = "Laura Díaz",
   Address = "Calle Laura",
 Photo = "laura.jpg",
            Birthday = DateTime.Parse("1988-04-12")
        },
 Images = new List<PropertyImage>(),
            Traces = new List<PropertyTrace>()
     }
   };

            _mockRepository.Setup(r => r.GetFilteredAsync(
          filter.Name,
        filter.Address,
       filter.MinPrice,
   filter.MaxPrice))
     .ReturnsAsync(properties);

            // Act
            var result = await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].PropertyName.Should().Contain("Apartamento");
        }

        [Test]
        public async Task GetFilteredPropertiesAsync_WithOnlyPriceRangeFilter_ReturnsPropertiesInRange()
        {
            // Arrange
            var filter = new PropertyFilterDto
            {
                MinPrice = 200000m,
                MaxPrice = 400000m
            };

            var properties = new List<Property>
      {
    new Property
           {
     Id = "507f1f77bcf86cd799439013",
        Name = "Casa en Rango",
   Address = "Calle Rango 1",
         Price = 300000m,
    CodeInternal = "PROP-600",
    Year = 2020,
   Owner = new Owner
     {
       OwnerId = "owner600",
        Name = "Roberto Silva",
     Address = "Calle Roberto",
         Photo = "roberto.jpg",
     Birthday = DateTime.Parse("1979-09-25")
             },
      Images = new List<PropertyImage>
    {
            new PropertyImage { ImageId = "img600", File = "range.jpg", Enabled = true }
        },
          Traces = new List<PropertyTrace>()
    }
            };

            _mockRepository.Setup(r => r.GetFilteredAsync(
            filter.Name,
                filter.Address,
     filter.MinPrice,
     filter.MaxPrice))
           .ReturnsAsync(properties);

            // Act
            var result = await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Price.Should().BeInRange(200000m, 400000m);
        }

        [Test]
        public void GetFilteredPropertiesAsync_WhenDatabaseExceptionOccurs_ThrowsDatabaseException()
        {
            // Arrange
            var filter = new PropertyFilterDto { Name = "Test" };

            _mockRepository.Setup(r => r.GetFilteredAsync(
                 It.IsAny<string>(),
                    It.IsAny<string>(),
           It.IsAny<decimal?>(),
          It.IsAny<decimal?>()))
           .ThrowsAsync(new DatabaseException("Database error"));

            // Act
            Func<Task> act = async () => await _service.GetFilteredPropertiesAsync(filter);

            // Assert
            act.Should().ThrowAsync<DatabaseException>();
        }

        #endregion
    }
}
