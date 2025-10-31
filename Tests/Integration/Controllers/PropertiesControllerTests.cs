using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using real_estate_api.Controllers;
using real_estate_api.Application.Services;
using real_estate_api.Application.DTO;
using real_estate_api.Application.Exceptions;

namespace real_estate_api.Tests.Integration.Controllers
{
    [TestFixture]
    public class PropertiesControllerTests
    {
   private Mock<IPropertyService> _mockService;
  private PropertiesController _controller;

        [SetUp]
 public void Setup()
 {
       _mockService = new Mock<IPropertyService>();
     _controller = new PropertiesController(_mockService.Object);
   }

        #region GetAll Tests

      [Test]
        public async Task GetAll_WhenPropertiesExist_ReturnsOkWithProperties()
        {
   // Arrange
var properties = new List<PropertyDto>
  {
       new PropertyDto
        {
      Id = "507f1f77bcf86cd799439011",
     IdOwner = "owner001",
    OwnerName = "Juan Pérez",
       PropertyName = "Casa Moderna",
    Address = "Calle 123",
         Price = 250000m,
   Image = "image1.jpg"
    },
 new PropertyDto
        {
    Id = "507f1f77bcf86cd799439012",
       IdOwner = "owner002",
      OwnerName = "María García",
    PropertyName = "Apartamento Centro",
   Address = "Avenida 456",
 Price = 180000m,
      Image = "image2.jpg"
       }
     };

            _mockService.Setup(s => s.GetAllPropertiesAsync())
     .ReturnsAsync(properties);

     // Act
   var result = await _controller.GetAll();

     // Assert
            result.Should().NotBeNull();
     result.Result.Should().BeOfType<OkObjectResult>();
 
var okResult = result.Result as OkObjectResult;
     okResult!.Value.Should().BeEquivalentTo(properties);
     }

      [Test]
        public async Task GetAll_WhenNoProperties_ReturnsOkWithEmptyList()
        {
      // Arrange
            _mockService.Setup(s => s.GetAllPropertiesAsync())
    .ReturnsAsync(new List<PropertyDto>());

      // Act
       var result = await _controller.GetAll();

  // Assert
  result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
       
     var okResult = result.Result as OkObjectResult;
  var properties = okResult!.Value as List<PropertyDto>;
properties.Should().NotBeNull();
      properties.Should().BeEmpty();
        }

      [Test]
   public async Task GetAll_CallsServiceMethod()
        {
// Arrange
        _mockService.Setup(s => s.GetAllPropertiesAsync())
     .ReturnsAsync(new List<PropertyDto>());

    // Act
 await _controller.GetAll();

// Assert
       _mockService.Verify(s => s.GetAllPropertiesAsync(), Times.Once);
     }

   #endregion

      #region GetById Tests

[Test]
        public async Task GetById_WithValidId_ReturnsOkWithProperty()
   {
// Arrange
     var propertyId = "507f1f77bcf86cd799439011";
         var propertyDetail = new PropertyDetailDto
     {
         Id = propertyId,
 Name = "Casa Detallada",
      Address = "Calle Principal 100",
   Price = 350000m,
       CodeInternal = "PROP-001",
       Year = 2022,
      OwnerId = "owner001",
      OwnerName = "Carlos López",
     OwnerAddress = "Calle Dueño 50",
    OwnerPhoto = "carlos.jpg",
  OwnerBirthday = DateTime.Parse("1980-05-15"),
 Images = new List<PropertyImageDto>
  {
     new PropertyImageDto { Id = "img001", File = "image1.jpg", Enabled = true }
     },
    Traces = new List<PropertyTraceDto>
        {
 new PropertyTraceDto
    {
       Id = "trace001",
    Name = "Venta",
      DateSale = DateTime.Parse("2022-01-15"),
        Value = 350000m,
          Tax = 17500m
      }
         }
      };

  _mockService.Setup(s => s.GetPropertyByIdAsync(propertyId))
          .ReturnsAsync(propertyDetail);

     // Act
var result = await _controller.GetById(propertyId);

   // Assert
            result.Should().NotBeNull();
     result.Result.Should().BeOfType<OkObjectResult>();
        
     var okResult = result.Result as OkObjectResult;
    okResult!.Value.Should().BeEquivalentTo(propertyDetail);
 }

     [Test]
     public async Task GetById_CallsServiceWithCorrectId()
     {
     // Arrange
      var propertyId = "507f1f77bcf86cd799439011";
         var propertyDetail = new PropertyDetailDto
       {
        Id = propertyId,
     Name = "Test Property",
  Address = "Test Address",
          Price = 100000m,
   CodeInternal = "TEST",
  Year = 2020,
 OwnerId = "owner001",
    OwnerName = "Test Owner",
  OwnerAddress = "Owner Address"
    };

      _mockService.Setup(s => s.GetPropertyByIdAsync(propertyId))
 .ReturnsAsync(propertyDetail);

        // Act
    await _controller.GetById(propertyId);

     // Assert
         _mockService.Verify(s => s.GetPropertyByIdAsync(propertyId), Times.Once);
        }

      [Test]
    public void GetById_WhenServiceThrowsNotFoundException_ExceptionPropagates()
        {
   // Arrange
       var propertyId = "507f1f77bcf86cd799439011";
   _mockService.Setup(s => s.GetPropertyByIdAsync(propertyId))
      .ThrowsAsync(new NotFoundException("Property", propertyId));

  // Act
            Func<Task> act = async () => await _controller.GetById(propertyId);

    // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }

 [Test]
        public void GetById_WhenServiceThrowsValidationException_ExceptionPropagates()
        {
   // Arrange
var propertyId = "";
       _mockService.Setup(s => s.GetPropertyByIdAsync(propertyId))
       .ThrowsAsync(new ValidationException("Id", "Property ID cannot be empty"));

    // Act
      Func<Task> act = async () => await _controller.GetById(propertyId);

// Assert
       act.Should().ThrowAsync<ValidationException>();
        }

[Test]
        public void GetById_WhenServiceThrowsDatabaseException_ExceptionPropagates()
        {
 // Arrange
       var propertyId = "507f1f77bcf86cd799439011";
_mockService.Setup(s => s.GetPropertyByIdAsync(propertyId))
    .ThrowsAsync(new DatabaseException("Database error"));

 // Act
      Func<Task> act = async () => await _controller.GetById(propertyId);

// Assert
       act.Should().ThrowAsync<DatabaseException>();
        }

    #endregion

   #region GetFiltered Tests

[Test]
        public async Task GetFiltered_WithValidFilter_ReturnsOkWithFilteredProperties()
        {
// Arrange
   var filter = new PropertyFilterDto
  {
     Name = "Casa",
           Address = "Calle",
    MinPrice = 100000m,
     MaxPrice = 500000m
        };

       var properties = new List<PropertyDto>
            {
      new PropertyDto
  {
    Id = "507f1f77bcf86cd799439011",
    IdOwner = "owner001",
    OwnerName = "Test Owner",
       PropertyName = "Casa Test",
          Address = "Calle Test 1",
          Price = 300000m,
    Image = "test.jpg"
   }
          };

   _mockService.Setup(s => s.GetFilteredPropertiesAsync(filter))
    .ReturnsAsync(properties);

     // Act
            var result = await _controller.GetFiltered(filter);

  // Assert
            result.Should().NotBeNull();
  result.Result.Should().BeOfType<OkObjectResult>();
          
       var okResult = result.Result as OkObjectResult;
       okResult!.Value.Should().BeEquivalentTo(properties);
     }

   [Test]
 public async Task GetFiltered_WithNameOnlyFilter_ReturnsMatchingProperties()
 {
    // Arrange
   var filter = new PropertyFilterDto
      {
      Name = "Apartamento"
        };

   var properties = new List<PropertyDto>
      {
    new PropertyDto
     {
Id = "507f1f77bcf86cd799439012",
 IdOwner = "owner002",
    OwnerName = "María García",
     PropertyName = "Apartamento Moderno",
       Address = "Torre Central",
      Price = 180000m,
  Image = "apt.jpg"
       }
     };

            _mockService.Setup(s => s.GetFilteredPropertiesAsync(It.Is<PropertyFilterDto>(
       f => f.Name == "Apartamento")))
      .ReturnsAsync(properties);

   // Act
    var result = await _controller.GetFiltered(filter);

     // Assert
            result.Should().NotBeNull();
   result.Result.Should().BeOfType<OkObjectResult>();
   
   var okResult = result.Result as OkObjectResult;
      var returnedProperties = okResult!.Value as List<PropertyDto>;
     returnedProperties.Should().HaveCount(1);
   returnedProperties![0].PropertyName.Should().Contain("Apartamento");
        }

   [Test]
        public async Task GetFiltered_WithPriceRangeFilter_ReturnsPropertiesInRange()
        {
  // Arrange
   var filter = new PropertyFilterDto
            {
     MinPrice = 200000m,
        MaxPrice = 400000m
            };

     var properties = new List<PropertyDto>
       {
      new PropertyDto
       {
      Id = "507f1f77bcf86cd799439013",
  IdOwner = "owner003",
        OwnerName = "Pedro Silva",
       PropertyName = "Casa en Rango",
      Address = "Calle Rango",
       Price = 300000m,
         Image = "range.jpg"
    }
       };

 _mockService.Setup(s => s.GetFilteredPropertiesAsync(It.Is<PropertyFilterDto>(
       f => f.MinPrice == 200000m && f.MaxPrice == 400000m)))
         .ReturnsAsync(properties);

    // Act
 var result = await _controller.GetFiltered(filter);

      // Assert
            result.Should().NotBeNull();
   var okResult = result.Result as OkObjectResult;
       var returnedProperties = okResult!.Value as List<PropertyDto>;
   returnedProperties![0].Price.Should().BeInRange(200000m, 400000m);
        }

     [Test]
  public async Task GetFiltered_WhenNoPropertiesMatch_ReturnsOkWithEmptyList()
        {
  // Arrange
     var filter = new PropertyFilterDto
   {
       Name = "NoExiste",
     MinPrice = 999999999m
        };

      _mockService.Setup(s => s.GetFilteredPropertiesAsync(filter))
 .ReturnsAsync(new List<PropertyDto>());

   // Act
            var result = await _controller.GetFiltered(filter);

            // Assert
result.Should().NotBeNull();
       result.Result.Should().BeOfType<OkObjectResult>();
         
     var okResult = result.Result as OkObjectResult;
  var properties = okResult!.Value as List<PropertyDto>;
         properties.Should().NotBeNull();
  properties.Should().BeEmpty();
        }

     [Test]
        public async Task GetFiltered_CallsServiceWithCorrectFilter()
  {
          // Arrange
       var filter = new PropertyFilterDto
       {
     Name = "Test",
         Address = "Address",
 MinPrice = 100000m,
      MaxPrice = 500000m
   };

       _mockService.Setup(s => s.GetFilteredPropertiesAsync(It.IsAny<PropertyFilterDto>()))
      .ReturnsAsync(new List<PropertyDto>());

         // Act
  await _controller.GetFiltered(filter);

// Assert
       _mockService.Verify(s => s.GetFilteredPropertiesAsync(It.Is<PropertyFilterDto>(
    f => f.Name == filter.Name &&
     f.Address == filter.Address &&
    f.MinPrice == filter.MinPrice &&
       f.MaxPrice == filter.MaxPrice)), Times.Once);
        }

   [Test]
        public void GetFiltered_WhenServiceThrowsValidationException_ExceptionPropagates()
        {
   // Arrange
var filter = new PropertyFilterDto
   {
     MinPrice = -1000m
        };

  _mockService.Setup(s => s.GetFilteredPropertiesAsync(filter))
      .ThrowsAsync(new ValidationException("MinPrice", "Cannot be negative"));

         // Act
   Func<Task> act = async () => await _controller.GetFiltered(filter);

    // Assert
       act.Should().ThrowAsync<ValidationException>();
        }

   [Test]
  public void GetFiltered_WhenServiceThrowsDatabaseException_ExceptionPropagates()
        {
  // Arrange
       var filter = new PropertyFilterDto { Name = "Test" };
    
  _mockService.Setup(s => s.GetFilteredPropertiesAsync(filter))
       .ThrowsAsync(new DatabaseException("Database error"));

    // Act
       Func<Task> act = async () => await _controller.GetFiltered(filter);

   // Assert
    act.Should().ThrowAsync<DatabaseException>();
        }

        #endregion
    }
}
