using Microsoft.AspNetCore.Mvc;
using real_estate_api.Application.DTO;
using real_estate_api.Application.Services;

namespace real_estate_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Get all properties with basic information
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<List<PropertyDto>>> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Get filtered properties by name, address, and price range
        /// </summary>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<List<PropertyDto>>> GetFiltered([FromQuery] PropertyFilterDto filter)
        {
            var properties = await _propertyService.GetFilteredPropertiesAsync(filter);
            return Ok(properties);
        }

        /// <summary>
        /// Get detailed information about a specific property
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<PropertyDetailDto>> GetById(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            return Ok(property);
        }
    }
}
