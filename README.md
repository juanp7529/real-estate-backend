# Real Estate API - Documentación

## ?? Descripción

API REST para gestión de propiedades inmobiliarias con MongoDB.

## ??? Estructura del Proyecto

```
real-estate-api/
??? Domain/
?   ??? Entities/
?       ??? Property.cs     # Entidad principal de propiedad
?       ??? Owner.cs          # Entidad de propietario
?       ??? PropertyImage.cs     # Entidad de imagen
?       ??? PropertyTrace.cs     # Entidad de historial
??? Application/
?   ??? DTO/
?   ?   ??? PropertyDto.cs    # DTO básico para listados
?   ?   ??? PropertyDetailDto.cs # DTO completo con todos los detalles
?   ?   ??? PropertyFilterDto.cs # DTO para filtros de búsqueda
?   ?   ??? PropertyImageDto.cs # DTO para datos de la imagen
?   ?   ??? PropertyTraceDto.cs # DTO para datos del historial
?   ??? Services/
?   ?   ??? PropertyService.cs   # Lógica de negocio
?   ??? Exceptions/
?    ??? NotFoundException.cs     # Excepción 404
?       ??? ValidationException.cs   # Excepción 400
?       ??? BusinessException.cs     # Excepción 422
?       ??? DatabaseException.cs     # Excepción 503
??? Infrastructure/
?   ??? Configuration/
?   ?   ??? MongoDbSettings.cs   # Configuración de MongoDB
?   ?   ??? MongoDbServiceExtensions.cs # Extensiones de configuración
?   ??? Repositories/
?   ?   ??? PropertyRepository.cs # Acceso a datos
?   ??? Middleware/
?       ??? ErrorHandlingMiddleware.cs # Middleware global de errores
??? Controllers/
  ??? PropertiesController.cs  # Endpoints de la API
```

## ?? Endpoints de la API

### 1. **Obtener todas las propiedades**
```http
GET /api/properties
```

**Respuesta exitosa (200):**
```json
[
  {
    "idOwner": "507f1f77bcf86cd799439011",
 "ownerName": "Juan Pérez",
    "propertyName": "Casa en la playa",
    "address": "Av. Principal 123",
    "price": 250000.00,
    "image": "https://example.com/image1.jpg"
  }
]
```

---

### 2. **Filtrar propiedades**
```http
GET /api/properties/filter?name={nombre}&address={direccion}&minPrice={precioMin}&maxPrice={precioMax}
```

**Parámetros de consulta (Query Parameters):**
- `name` (opcional): Busca por nombre de propiedad (case-insensitive)
- `address` (opcional): Busca por dirección (case-insensitive)
- `minPrice` (opcional): Precio mínimo
- `maxPrice` (opcional): Precio máximo

**Ejemplo de uso:**
```http
GET /api/properties/filter?name=casa&minPrice=100000&maxPrice=500000
```

**Respuesta exitosa (200):**
```json
[
  {
    "idOwner": "507f1f77bcf86cd799439011",
    "ownerName": "Juan Pérez",
    "propertyName": "Casa moderna",
    "address": "Calle 45 #12-34",
    "price": 350000.00,
    "image": "https://example.com/casa.jpg"
  }
]
```

---

### 3. **Obtener detalles de una propiedad**
```http
GET /api/properties/{id}
```

**Parámetros de ruta:**
- `id`: ID de la propiedad (ObjectId de MongoDB)

**Ejemplo de uso:**
```http
GET /api/properties/507f1f77bcf86cd799439011
```

**Respuesta exitosa (200):**
```json
{
  "id": "507f1f77bcf86cd799439011",
  "name": "Casa moderna",
  "address": "Calle 45 #12-34",
  "price": 350000.00,
  "codeInternal": "PROP-001",
  "year": 2020,
  "ownerId": "507f191e810c19729de860ea",
  "ownerName": "Juan Pérez",
  "ownerAddress": "Calle 10 #20-30",
  "ownerPhoto": "https://example.com/owner.jpg",
  "ownerBirthday": "1985-05-15T00:00:00Z",
  "images": [
    {
      "id": "img001",
      "file": "https://example.com/image1.jpg",
    "enabled": true
    },
    {
      "id": "img002",
      "file": "https://example.com/image2.jpg",
    "enabled": true
    }
  ],
  "traces": [
    {
      "id": "trace001",
      "name": "Venta inicial",
      "dateSale": "2020-03-15T00:00:00Z",
      "value": 350000.00,
   "tax": 17500.00
    }
  ]
}
```

**Respuesta de error (404):**
```json
{
  "statusCode": 404,
  "type": "Not Found",
  "message": "Property with id '507f1f77bcf86cd799439011' was not found.",
  "traceId": "0HN0Q2LH3OKFM:00000001",
"timestamp": "2024-01-15T10:30:00.000Z"
}
```

**Otros códigos de error posibles:**
- `400 Bad Request`: Validación de entrada fallida
- `422 Unprocessable Entity`: Violación de reglas de negocio
- `503 Service Unavailable`: Error de base de datos

?? **Ver documentación completa de manejo de errores**: [ERROR_HANDLING.md](ERROR_HANDLING.md)

---

## ?? Configuración

### appsettings.json
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RealEstateDb"
  }
}
```

### Requisitos
- .NET 8
- MongoDB 4.0+
- Paquetes NuGet:
  - MongoDB.Driver 3.5.0
  - Swashbuckle.AspNetCore 9.0.6

---

## ?? Características Implementadas

? **DTOs definidos:**
- `PropertyDto`: Información básica (IdOwner, Nombre, Dirección, Precio, Imagen)
- `PropertyDetailDto`: Información completa incluyendo propietario, imágenes y historial
- `PropertyFilterDto`: Filtros de búsqueda

? **Endpoints:**
- Lista de propiedades con información básica
- Filtros de búsqueda por:
  - Nombre de propiedad (búsqueda parcial, case-insensitive)
  - Dirección (búsqueda parcial, case-insensitive)
  - Rango de precios (mínimo y máximo)
- Detalles completos de cada propiedad

? **Arquitectura:**
- Separación de capas (Domain, Application, Infrastructure)
- Patrón Repository
- Inyección de dependencias
- Configuración con Options Pattern
- Extension Methods para configuración modular (MongoDB)
- **Manejo Global de Errores con Middleware**
- **Excepciones Personalizadas por tipo de error**
- **Controllers limpios sin try-catch**
- **Logging estructurado con contexto**

---

## ?? Probar la API

### Con Swagger UI
1. Ejecuta la aplicación
2. Navega a: `https://localhost:{puerto}/swagger`
3. Prueba los endpoints desde la interfaz

### Con cURL

**Obtener todas las propiedades:**
```bash
curl -X GET "https://localhost:{puerto}/api/properties"
```

**Filtrar propiedades:**
```bash
curl -X GET "https://localhost:{puerto}/api/properties/filter?name=casa&minPrice=100000&maxPrice=500000"
```

**Obtener detalles:**
```bash
curl -X GET "https://localhost:{puerto}/api/properties/507f1f77bcf86cd799439011"
```

---

## ?? Modelo de Datos

### Property (Colección: Properties)
```javascript
{
  "_id": ObjectId("..."),
  "Name": "string",
  "Address": "string",
  "Price": Decimal128,
  "CodeInternal": "string",
  "Year": int,
  "Owner": {
    "Id": "string",
    "Name": "string",
    "Address": "string",
    "Photo": "string",
"Birthday": ISODate
  },
  "Images": [
    {
      "Id": "string",
      "File": "string",
   "Enabled": bool
    }
  ],
  "Traces": [
    {
   "Id": "string",
      "Name": "string",
      "DateSale": ISODate,
      "Value": Decimal128,
  "Tax": Decimal128
    }
  ]
}
```

---

## ?? Características de Búsqueda

- **Búsqueda por nombre**: Utiliza regex case-insensitive para búsquedas parciales
- **Búsqueda por dirección**: Utiliza regex case-insensitive para búsquedas parciales
- **Filtro de precio**: Permite establecer rangos mínimos y máximos
- **Combinación de filtros**: Todos los filtros pueden usarse simultáneamente

---

## ?? Imagen destacada

El DTO básico (`PropertyDto`) retorna automáticamente la primera imagen habilitada de la propiedad, si existe.

---

## ?? Notas adicionales

- Los IDs de MongoDB se manejan como strings (ObjectId serializado)
- Todas las búsquedas de texto son case-insensitive
- Los precios se manejan como `decimal` para precisión
- Las imágenes y trazas son listas que pueden estar vacías dependiendo lo cargado en MongoDB

---

## ?? Manejo de Errores

La API utiliza excepciones personalizadas para el manejo de errores. Algunas de las excepciones incluyen:

- `NotFoundException`: Para errores 404 - Recurso no encontrado
- `ValidationException`: Para errores 400 - Solicitudes incorrectas
- `BusinessException`: Para errores 422 - Reglas de negocio no cumplidas
- `DatabaseException`: Para errores 503 - Problemas con la base de datos

Además, se implementa un middleware global `ErrorHandlingMiddleware` para capturar y formatear errores de manera uniforme.

---
