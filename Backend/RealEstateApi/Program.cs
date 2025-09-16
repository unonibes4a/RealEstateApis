using MongoDB.Driver;
using RealEstateApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    Console.WriteLine($"Conectando a MongoDB: {connectionString}");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var databaseName = "baseDeDatosIni";
    Console.WriteLine($"Configurando base de datos: {databaseName}");
    return client.GetDatabase(databaseName);
});

// Registrar todos los servicios
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<PropertyImageService>();
builder.Services.AddScoped<PropertyTraceService>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("*");
    });
    
    // Política específica para desarrollo
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://127.0.0.1:3000",
            "http://localhost:8080",
            "http://127.0.0.1:8080",
            "file://",
            "null"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// 🔧 VERIFICAR Y CREAR BASE DE DATOS
using (var scope = app.Services.CreateScope())
{
    try
    {
        var client = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        
        Console.WriteLine("🔍 Verificando conexión a MongoDB...");
        
        // Verificar conexión haciendo ping
        await client.GetDatabase("admin").RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
        Console.WriteLine("✅ Conexión a MongoDB exitosa");
        
        // Listar bases de datos existentes
        var existingDatabases = await client.ListDatabaseNamesAsync();
        var dbList = await existingDatabases.ToListAsync();
        Console.WriteLine($"📊 Bases de datos existentes: {string.Join(", ", dbList)}");
        
        // Verificar si nuestra base de datos existe
        var databaseName = database.DatabaseNamespace.DatabaseName;
        if (!dbList.Contains(databaseName))
        {
            Console.WriteLine($"🆕 Base de datos '{databaseName}' no existe, se creará automáticamente");
        }
        
        // 🌱 EJECUTAR SEEDING (esto creará la base de datos si no existe)
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedDataAsync();
        
        // Verificar que se creó la base de datos
        var updatedDatabases = await client.ListDatabaseNamesAsync();
        var updatedDbList = await updatedDatabases.ToListAsync();
        
        if (updatedDbList.Contains(databaseName))
        {
            Console.WriteLine($"✅ Base de datos '{databaseName}' creada y configurada correctamente");
            
            // Listar colecciones creadas
            var collections = await database.ListCollectionNamesAsync();
            var collectionList = await collections.ToListAsync();
            Console.WriteLine($"📋 Colecciones creadas: {string.Join(", ", collectionList)}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error configurando base de datos: {ex.Message}");
        Console.WriteLine($"🔧 Stack trace: {ex.StackTrace}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development"); // Usar política específica en desarrollo
}
else
{
    app.UseCors(); // Usar política por defecto en producción
}
app.UseAuthorization();
app.MapControllers();

// Endpoint para verificar datos y forzar creación si es necesario
app.MapGet("/test-mongo", async (IMongoDatabase database) =>
{
    try
    {
        var collections = await database.ListCollectionNamesAsync();
        var collectionList = await collections.ToListAsync();
        
        // Si no hay colecciones, ejecutar seeding manualmente
        if (!collectionList.Any())
        {
            Console.WriteLine("⚠️ No se encontraron colecciones, ejecutando seeding...");
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedDataAsync();
            
            // Volver a obtener las colecciones
            collections = await database.ListCollectionNamesAsync();
            collectionList = await collections.ToListAsync();
        }
        
        return Results.Ok(new
        {
            message = "MongoDB conectado correctamente",
            database = database.DatabaseNamespace.DatabaseName,
            collections = collectionList,
            status = collectionList.Any() ? "Base de datos con datos" : "Base de datos vacía"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            message = "Error de conexión a MongoDB",
            error = ex.Message,
            details = ex.StackTrace
        });
    }
});

// Endpoint para forzar recreación de datos
app.MapPost("/recreate-database", async (IMongoDatabase database) =>
{
    try
    {
        Console.WriteLine("🔄 Recreando base de datos...");
        
        // Borrar todas las colecciones
        var collections = await database.ListCollectionNamesAsync();
        var collectionList = await collections.ToListAsync();
        
        foreach (var collectionName in collectionList)
        {
            await database.DropCollectionAsync(collectionName);
            Console.WriteLine($"🗑️ Colección '{collectionName}' eliminada");
        }
        
        // Ejecutar seeding
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedDataAsync();
        
        return Results.Ok(new
        {
            message = "Base de datos recreada exitosamente",
            database = database.DatabaseNamespace.DatabaseName
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            message = "Error recreando base de datos",
            error = ex.Message
        });
    }
});

app.Run();