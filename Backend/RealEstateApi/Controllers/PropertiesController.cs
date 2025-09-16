using Microsoft.AspNetCore.Mvc;
using RealEstateApi.DTOs;
using RealEstateApi.Services;
using RealEstateApi.Models;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RealEstateController : ControllerBase
    {
        private readonly PropertyService _propertyService;
        private readonly OwnerService _ownerService;
        private readonly PropertyImageService _propertyImageService;
        private readonly PropertyTraceService _propertyTraceService;
        private readonly DatabaseSeeder _databaseSeeder;

        public RealEstateController(
            PropertyService propertyService,
            OwnerService ownerService,
            PropertyImageService propertyImageService,
            PropertyTraceService propertyTraceService,
            DatabaseSeeder databaseSeeder)
        {
            _propertyService = propertyService;
            _ownerService = ownerService;
            _propertyImageService = propertyImageService;
            _propertyTraceService = propertyTraceService;
            _databaseSeeder = databaseSeeder;
        }

        [HttpGet("properties/hello")]
        public IActionResult Hello()
        {
            return Ok(new { 
                message = "¡Hola mundo desde la API de Real Estate!", 
                timestamp = DateTime.Now,
                version = "1.0"
            });
        }

        [HttpGet("properties")]
        public async Task<ActionResult<List<PropertyDto>>> GetProperties([FromQuery] PropertyFilterDto filter)
        {
            try
            {
                var properties = await _propertyService.GetAllPropertiesAsync(filter);
                return Ok(new {
                    success = true,
                    count = properties.Count,
                    data = properties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error obteniendo propiedades", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("properties/{id}")]
        public async Task<ActionResult<PropertyDto>> GetProperty(string id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                
                if (property == null)
                    return NotFound(new { 
                        success = false, 
                        message = $"Propiedad con ID {id} no encontrada" 
                    });
                
                return Ok(new {
                    success = true,
                    data = property
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error obteniendo propiedad", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("properties/count")]
        public async Task<IActionResult> GetPropertiesCount()
        {
            try
            {
                var count = await _propertyService.GetCountAsync();
                return Ok(new {
                    success = true,
                    totalProperties = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error contando propiedades", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("owners")]
        public async Task<ActionResult<List<Owner>>> GetOwners()
        {
            try
            {
                var owners = await _ownerService.GetAllAsync();
                return Ok(new {
                    success = true,
                    count = owners.Count,
                    data = owners
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error obteniendo propietarios", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("owners/count")]
        public async Task<IActionResult> GetOwnersCount()
        {
            try
            {
                var count = await _ownerService.GetCountAsync();
                return Ok(new {
                    success = true,
                    totalOwners = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error contando propietarios", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("property-images/count")]
        public async Task<IActionResult> GetPropertyImagesCount()
        {
            try
            {
                var count = await _propertyImageService.GetCountAsync();
                return Ok(new {
                    success = true,
                    totalImages = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error contando imágenes", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("property-traces/count")]
        public async Task<IActionResult> GetPropertyTracesCount()
        {
            try
            {
                var count = await _propertyTraceService.GetCountAsync();
                return Ok(new {
                    success = true,
                    totalTraces = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error contando trazas", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetDatabaseStatus()
        {
            try
            {
                var propertiesCount = await _propertyService.GetCountAsync();
                var ownersCount = await _ownerService.GetCountAsync();
                var imagesCount = await _propertyImageService.GetCountAsync();
                var tracesCount = await _propertyTraceService.GetCountAsync();

                return Ok(new {
                    success = true,
                    database = "baseDeDatosIni",
                    timestamp = DateTime.Now,
                    collections = new {
                        properties = propertiesCount,
                        owners = ownersCount,
                        propertyImages = imagesCount,
                        propertyTraces = tracesCount
                    },
                    totalRecords = propertiesCount + ownersCount + imagesCount + tracesCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error obteniendo estado de la base de datos", 
                    error = ex.Message 
                });
            }
        }

        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await _databaseSeeder.SeedDataAsync();
                return Ok(new {
                    success = true,
                    message = "Datos de prueba creados exitosamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error creando datos de prueba", 
                    error = ex.Message 
                });
            }
        }

        [HttpDelete("clear-data")]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                return Ok(new {
                    success = true,
                    message = "Funcionalidad de limpieza no implementada aún",
                    note = "Para implementar, agregar métodos DeleteAll en cada servicio"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error limpiando datos", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("reports/summary")]
        public async Task<IActionResult> GetSummaryReport()
        {
            try
            {
                var propertiesCount = await _propertyService.GetCountAsync();
                var ownersCount = await _ownerService.GetCountAsync();
                var imagesCount = await _propertyImageService.GetCountAsync();
                var tracesCount = await _propertyTraceService.GetCountAsync();

                var allProperties = await _propertyService.GetAllPropertiesAsync(new PropertyFilterDto());
                var avgPrice = allProperties.Any() ? allProperties.Average(p => p.Price) : 0;
                var maxPrice = allProperties.Any() ? allProperties.Max(p => p.Price) : 0;
                var minPrice = allProperties.Any() ? allProperties.Min(p => p.Price) : 0;

                return Ok(new {
                    success = true,
                    summary = new {
                        totalProperties = propertiesCount,
                        totalOwners = ownersCount,
                        totalImages = imagesCount,
                        totalTraces = tracesCount,
                        priceStatistics = new {
                            average = avgPrice,
                            maximum = maxPrice,
                            minimum = minPrice,
                            currency = "COP"
                        }
                    },
                    generatedAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error generando reporte", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("search/expensive")]
        public async Task<IActionResult> GetExpensiveProperties([FromQuery] int limit = 5)
        {
            try
            {
                var allProperties = await _propertyService.GetAllPropertiesAsync(new PropertyFilterDto());
                var expensiveProperties = allProperties
                    .OrderByDescending(p => p.Price)
                    .Take(limit)
                    .ToList();

                return Ok(new {
                    success = true,
                    count = expensiveProperties.Count,
                    data = expensiveProperties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error obteniendo propiedades costosas", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("search/by-year/{year}")]
        public async Task<IActionResult> GetPropertiesByYear(int year)
        {
            try
            {
                var filter = new PropertyFilterDto { Year = year };
                var properties = await _propertyService.GetAllPropertiesAsync(filter);

                return Ok(new {
                    success = true,
                    year = year,
                    count = properties.Count,
                    data = properties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error obteniendo propiedades del año {year}", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("search/price-range")]
        public async Task<IActionResult> GetPropertiesByPriceRange([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            try
            {
                var filter = new PropertyFilterDto 
                { 
                    MinPrice = minPrice, 
                    MaxPrice = maxPrice 
                };
                var properties = await _propertyService.GetAllPropertiesAsync(filter);

                return Ok(new {
                    success = true,
                    priceRange = new { min = minPrice, max = maxPrice },
                    count = properties.Count,
                    data = properties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error filtrando por rango de precios", 
                    error = ex.Message 
                });
            }
        }
    }
}