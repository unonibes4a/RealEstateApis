using MongoDB.Driver;
using RealEstateApi.Models;
using RealEstateApi.DTOs;

namespace RealEstateApi.Services
{
    public class PropertyService
    {
        private readonly IMongoCollection<Property> _properties;

        public PropertyService(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
        }

        public async Task<List<PropertyDto>> GetAllPropertiesAsync(PropertyFilterDto filter)
        {
            var filterBuilder = Builders<Property>.Filter;
            var mongoFilter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(filter.Name))
                mongoFilter &= filterBuilder.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(filter.Name, "i"));

            if (!string.IsNullOrEmpty(filter.Address))
                mongoFilter &= filterBuilder.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(filter.Address, "i"));

            if (filter.MinPrice.HasValue)
                mongoFilter &= filterBuilder.Gte(x => x.Price, filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                mongoFilter &= filterBuilder.Lte(x => x.Price, filter.MaxPrice.Value);

            if (filter.Year.HasValue)
                mongoFilter &= filterBuilder.Eq(x => x.Year, filter.Year.Value);

            var properties = await _properties.Find(mongoFilter).ToListAsync();

            return properties.Select(p => new PropertyDto
            {
                Id = p.Id,
                IdOwner = p.IdOwner,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                CodeInternal = p.CodeInternal,
                Year = p.Year,
                Image = p.Image
            }).ToList();
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(string id)
        {
            var property = await _properties.Find(x => x.Id == id).FirstOrDefaultAsync();
            
            if (property == null) return null;

            return new PropertyDto
            {
                Id = property.Id,
                IdOwner = property.IdOwner,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                Image = property.Image
            };
        }

        public async Task<long> GetCountAsync()
        {
            return await _properties.CountDocumentsAsync(FilterDefinition<Property>.Empty);
        }

        public async Task CreateManyAsync(List<Property> properties)
        {
            await _properties.InsertManyAsync(properties);
        }
    }

    public class OwnerService
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerService(IMongoDatabase database)
        {
            _owners = database.GetCollection<Owner>("Owners");
        }

        public async Task<long> GetCountAsync()
        {
            return await _owners.CountDocumentsAsync(FilterDefinition<Owner>.Empty);
        }

        public async Task CreateManyAsync(List<Owner> owners)
        {
            await _owners.InsertManyAsync(owners);
        }

        public async Task<List<Owner>> GetAllAsync()
        {
            return await _owners.Find(FilterDefinition<Owner>.Empty).ToListAsync();
        }
    }

    public class PropertyImageService
    {
        private readonly IMongoCollection<PropertyImage> _propertyImages;

        public PropertyImageService(IMongoDatabase database)
        {
            _propertyImages = database.GetCollection<PropertyImage>("PropertyImages");
        }

        public async Task<long> GetCountAsync()
        {
            return await _propertyImages.CountDocumentsAsync(FilterDefinition<PropertyImage>.Empty);
        }

        public async Task CreateManyAsync(List<PropertyImage> propertyImages)
        {
            await _propertyImages.InsertManyAsync(propertyImages);
        }
    }

    public class PropertyTraceService
    {
        private readonly IMongoCollection<PropertyTrace> _propertyTraces;

        public PropertyTraceService(IMongoDatabase database)
        {
            _propertyTraces = database.GetCollection<PropertyTrace>("PropertyTraces");
        }

        public async Task<long> GetCountAsync()
        {
            return await _propertyTraces.CountDocumentsAsync(FilterDefinition<PropertyTrace>.Empty);
        }

        public async Task CreateManyAsync(List<PropertyTrace> propertyTraces)
        {
            await _propertyTraces.InsertManyAsync(propertyTraces);
        }
    }

    public class DatabaseSeeder
    {
        private readonly OwnerService _ownerService;
        private readonly PropertyService _propertyService;
        private readonly PropertyImageService _propertyImageService;
        private readonly PropertyTraceService _propertyTraceService;

        public DatabaseSeeder(
            OwnerService ownerService,
            PropertyService propertyService,
            PropertyImageService propertyImageService,
            PropertyTraceService propertyTraceService)
        {
            _ownerService = ownerService;
            _propertyService = propertyService;
            _propertyImageService = propertyImageService;
            _propertyTraceService = propertyTraceService;
        }

        public async Task SeedDataAsync()
        {
            Console.WriteLine("🌱 Iniciando proceso de seeding...");

            // Verificar si ya hay datos
            var ownersCount = await _ownerService.GetCountAsync();
            if (ownersCount > 0)
            {
                Console.WriteLine("✅ La base de datos ya tiene datos, omitiendo seeding.");
                return;
            }

            // 1. Crear Owners
            var owners = new List<Owner>
            {
                new Owner
                {
                    Name = "Juan Pérez",
                    Address = "Calle 10 #15-20, Bogotá",
                    Photo = "juan_perez.jpg",
                    Birthday = new DateTime(1980, 5, 15)
                },
                new Owner
                {
                    Name = "María García",
                    Address = "Carrera 25 #30-45, Medellín", 
                    Photo = "maria_garcia.jpg",
                    Birthday = new DateTime(1975, 8, 22)
                },
                new Owner
                {
                    Name = "Carlos López",
                    Address = "Avenida 68 #45-12, Cali",
                    Photo = "carlos_lopez.jpg",
                    Birthday = new DateTime(1985, 12, 3)
                }
            };

            await _ownerService.CreateManyAsync(owners);
            Console.WriteLine($"✅ Creados {owners.Count} propietarios");

            // Obtener los IDs de los owners creados
            var createdOwners = await _ownerService.GetAllAsync();

            // 2. Crear Properties
            var properties = new List<Property>
            {
                new Property
                {
                    IdOwner = createdOwners[0].Id!,
                    Name = "Casa Campestre en La Sabana",
                    Address = "Vereda La Esperanza, Chía",
                    Price = 450000000,
                    CodeInternal = "PROP001",
                    Year = 2020,
                    Image = "casa_campestre.jpg"
                },
                new Property
                {
                    IdOwner = createdOwners[1].Id!,
                    Name = "Apartamento Zona Rosa",
                    Address = "Carrera 13 #85-32, Bogotá",
                    Price = 320000000,
                    CodeInternal = "PROP002", 
                    Year = 2019,
                    Image = "apto_zona_rosa.jpg"
                },
                new Property
                {
                    IdOwner = createdOwners[0].Id!,
                    Name = "Casa en Cedritos",
                    Address = "Calle 147 #45-67, Bogotá",
                    Price = 280000000,
                    CodeInternal = "PROP003",
                    Year = 2021,
                    Image = "casa_cedritos.jpg"
                },
                new Property
                {
                    IdOwner = createdOwners[2].Id!,
                    Name = "Penthouse El Poblado",
                    Address = "Carrera 43A #15-25, Medellín",
                    Price = 850000000,
                    CodeInternal = "PROP004",
                    Year = 2022,
                    Image = "penthouse_poblado.jpg"
                },
                new Property
                {
                    IdOwner = createdOwners[1].Id!,
                    Name = "Apartaestudio Chapinero",
                    Address = "Carrera 15 #63-45, Bogotá",
                    Price = 180000000,
                    CodeInternal = "PROP005",
                    Year = 2018,
                    Image = "apartaestudio_chapinero.jpg"
                }
            };

            await _propertyService.CreateManyAsync(properties);
            Console.WriteLine($"✅ Creadas {properties.Count} propiedades");

            // 3. Crear PropertyImages
            var propertyImages = new List<PropertyImage>
            {
                new PropertyImage { IdProperty = properties[0].Id!, File = "casa_campestre_1.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[0].Id!, File = "casa_campestre_2.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[1].Id!, File = "apto_zona_rosa_1.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[1].Id!, File = "apto_zona_rosa_2.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[2].Id!, File = "casa_cedritos_1.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[3].Id!, File = "penthouse_1.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[3].Id!, File = "penthouse_2.jpg", Enabled = true },
                new PropertyImage { IdProperty = properties[4].Id!, File = "apartaestudio_1.jpg", Enabled = true }
            };

            await _propertyImageService.CreateManyAsync(propertyImages);
            Console.WriteLine($"✅ Creadas {propertyImages.Count} imágenes de propiedades");

            // 4. Crear PropertyTraces
            var propertyTraces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdProperty = properties[0].Id!,
                    DateSale = DateTime.Now.AddDays(-30),
                    Name = "Venta Inicial",
                    Value = 450000000,
                    Tax = 22500000
                },
                new PropertyTrace
                {
                    IdProperty = properties[1].Id!,
                    DateSale = DateTime.Now.AddDays(-45),
                    Name = "Compra",
                    Value = 320000000,
                    Tax = 16000000
                },
                new PropertyTrace
                {
                    IdProperty = properties[2].Id!,
                    DateSale = DateTime.Now.AddDays(-60),
                    Name = "Transferencia",
                    Value = 280000000,
                    Tax = 14000000
                },
                new PropertyTrace
                {
                    IdProperty = properties[3].Id!,
                    DateSale = DateTime.Now.AddDays(-15),
                    Name = "Venta Premium",
                    Value = 850000000,
                    Tax = 42500000
                }
            };

            await _propertyTraceService.CreateManyAsync(propertyTraces);
            Console.WriteLine($"✅ Creados {propertyTraces.Count} registros de trazabilidad");

            Console.WriteLine("🎉 ¡Seeding completado exitosamente!");
        }
    }
}