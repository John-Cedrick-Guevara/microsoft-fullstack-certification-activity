# InventoryHub API Documentation

## Overview
This document provides complete API documentation for the InventoryHub backend service.

---

## Base URL
```
Development: https://localhost:5001
Production: [To be configured]
```

---

## Endpoints

### 1. Get Product List

**Endpoint:** `GET /api/productlist`

**Description:** Retrieves the complete product catalog with category information.

**Authentication:** None (public endpoint)

**CORS:** Enabled (AllowAnyOrigin)

**Caching:** 5-minute in-memory cache

---

#### Request

**Method:** `GET`

**Headers:**
```
Accept: application/json
```

**Query Parameters:** None

**Request Body:** None

---

#### Response

**Success Response:**

**Status Code:** `200 OK`

**Content-Type:** `application/json`

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "price": 1200.50,
    "stock": 25,
    "category": {
      "id": 101,
      "name": "Electronics"
    }
  },
  {
    "id": 2,
    "name": "Headphones",
    "price": 50.00,
    "stock": 100,
    "category": {
      "id": 102,
      "name": "Accessories"
    }
  },
  {
    "id": 3,
    "name": "Wireless Mouse",
    "price": 25.99,
    "stock": 150,
    "category": {
      "id": 102,
      "name": "Accessories"
    }
  },
  {
    "id": 4,
    "name": "USB-C Hub",
    "price": 45.00,
    "stock": 75,
    "category": {
      "id": 101,
      "name": "Electronics"
    }
  }
]
```

---

**Error Response:**

**Status Code:** `500 Internal Server Error`

**Content-Type:** `application/problem+json`

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Error retrieving product list",
  "status": 500,
  "detail": "[Error message details]"
}
```

---

#### Response Schema

**ProductDto Object:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `id` | integer | Unique product identifier | Yes |
| `name` | string | Product name/title | Yes |
| `price` | number | Price in USD (decimal) | Yes |
| `stock` | integer | Available inventory quantity | Yes |
| `category` | CategoryDto | Nested category object | Yes |

**CategoryDto Object:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `id` | integer | Unique category identifier | Yes |
| `name` | string | Category name | Yes |

---

#### Example Usage

**cURL:**
```bash
curl -X GET "https://localhost:5001/api/productlist" \
     -H "Accept: application/json"
```

**JavaScript (Fetch API):**
```javascript
fetch('https://localhost:5001/api/productlist')
  .then(response => response.json())
  .then(products => console.log(products))
  .catch(error => console.error('Error:', error));
```

**C# (HttpClient):**
```csharp
using var client = new HttpClient();
var response = await client.GetAsync("https://localhost:5001/api/productlist");
response.EnsureSuccessStatusCode();

var json = await response.Content.ReadAsStringAsync();
var products = JsonSerializer.Deserialize<ProductDto[]>(json, 
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
```

**PowerShell:**
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/productlist" -Method Get
```

---

#### Performance Characteristics

**First Request (Cache Miss):**
- Response Time: ~50ms
- Includes data generation and JSON serialization

**Subsequent Requests (Cache Hit):**
- Response Time: ~5ms
- Data served from in-memory cache

**Cache Duration:** 5 minutes

**Cache Invalidation:** Automatic (time-based)

---

## Data Models

### ProductDto (C# Backend)
```csharp
record ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
    public CategoryDto Category { get; set; } = null!;
}
```

### CategoryDto (C# Backend)
```csharp
record CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### Product (C# Frontend)
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
    public Category Category { get; set; } = null!;
}
```

### Category (C# Frontend)
```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

---

## CORS Configuration

**Allowed Origins:** `*` (all origins)  
**Allowed Methods:** `GET, POST, PUT, DELETE, OPTIONS`  
**Allowed Headers:** `*` (all headers)

**Note:** In production, restrict origins to specific domains for security.

---

## Error Handling

The API uses standard HTTP status codes:

| Status Code | Meaning | When It Occurs |
|-------------|---------|----------------|
| 200 | OK | Successful request |
| 500 | Internal Server Error | Server-side error (logged) |

**Error Response Format:**
- Follows RFC 7807 Problem Details standard
- Includes `type`, `title`, `status`, and `detail` fields
- Suitable for debugging and user display

---

## Rate Limiting

**Current Status:** Not implemented  
**Recommended for Production:** 100 requests per minute per IP

---

## Versioning

**Current Version:** v1 (implicit)  
**Versioning Strategy:** URL path versioning (future: `/api/v1/productlist`, `/api/v2/productlist`)

---

## Security Considerations

### Current Implementation (Development)
- ✅ HTTPS enabled
- ✅ CORS configured
- ⚠️ No authentication (public endpoint)
- ⚠️ No rate limiting
- ⚠️ AllowAnyOrigin CORS policy

### Production Recommendations
- 🔐 Implement JWT authentication
- 🔐 Restrict CORS to specific domains
- 🔐 Add rate limiting middleware
- 🔐 Use API keys for client identification
- 🔐 Enable request logging
- 🔐 Implement input validation

---

## Testing the API

### Using Browser
Navigate to: `https://localhost:5001/api/productlist`

### Using Postman
1. Create new GET request
2. URL: `https://localhost:5001/api/productlist`
3. Send request
4. View JSON response

### Using VS Code REST Client
Create file `test.http`:
```http
### Get all products
GET https://localhost:5001/api/productlist
Accept: application/json
```

---

## OpenAPI/Swagger

**Swagger UI (Development Only):**
```
https://localhost:5001/swagger
```

**OpenAPI Spec:**
```
https://localhost:5001/swagger/v1/swagger.json
```

---

## Monitoring & Observability

### Recommended Tools
- **Application Insights** - Azure monitoring
- **Seq** - Structured logging
- **Prometheus + Grafana** - Metrics and dashboards

### Key Metrics to Track
- Request count per endpoint
- Average response time
- Cache hit rate
- Error rate
- CPU and memory usage

---

## Change Log

### Version 1.0 (Current)
- ✅ Initial release
- ✅ GET /api/productlist endpoint
- ✅ In-memory caching
- ✅ CORS support
- ✅ Nested category structure
- ✅ camelCase JSON serialization

### Planned Features (v1.1)
- 🔜 POST endpoint for adding products
- 🔜 PUT endpoint for updating products
- 🔜 DELETE endpoint for removing products
- 🔜 Search and filter functionality
- 🔜 Pagination support

---

## Support & Contact

**Issues:** Report via GitHub Issues  
**Documentation:** This file + REFLECTION.md  
**Source Code:** Available in project repository

---

*Last Updated: November 18, 2025*  
*API Version: 1.0*  
*Document Version: 1.0*
