# FishSpinDays API Documentation

A RESTful API for FishSpinDays app.

## Authentication

This API use JWT Bearer token authentication. To access the protected endpoints you must include the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Get JWT Token

```http
POST /api/Auth
Content-Type: application/json

{
  "username": "your-username",
  "password": "your-password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 86400,
  "tokenType": "Bearer"
}
```

## Publications API

### Get All Publications
```http
GET /api/publications?page=1
```

### Get Publication by ID
```http
GET /api/publications/{id}
```

### Create New Publication (Requires Auth)
```http
POST /api/publications
Content-Type: application/json
Authorization: Bearer <token>

{
  "title": "Publication Title",
  "description": "Publication content with HTML",
  "section": "Sea fishing"
}
```

### Like Publication (Requires Auth)
```http
POST /api/publications/{id}/like
Authorization: Bearer <token>
```

### Get Most Rated Publication
```http
GET /api/publications/most-rated
```

### Get Publications by Category

#### Sea Publications
```http
GET /api/publications/sea?page=1
```

#### Freshwater Publications
```http
GET /api/publications/freshwater?page=1
```

#### Section-Specific Publications
```http
GET /api/publications/sections/rods
GET /api/publications/sections/lures
GET /api/publications/sections/handmade
GET /api/publications/sections/eco
GET /api/publications/sections/school
GET /api/publications/sections/anti
GET /api/publications/sections/breeding
```

### Time-Based Publications
```http
GET /api/publications/year/2024
GET /api/publications/month/10
```

### Search Publications
```http
GET /api/publications/search?searchTerm=fishing
```

### Get Publication Statistics
```http
GET /api/publications/stats
```

## Comments API

### Add Comment (Requires Auth)
```http
POST /api/comments
Content-Type: application/json
Authorization: Bearer <token>

{
  "publicationId": 123,
  "text": "Great article about fishing!"
}
```

### Get Comment by ID
```http
GET /api/comments/{id}
```

### Like Comment (Requires Auth)
```http
POST /api/comments/{id}/like
Authorization: Bearer <token>
```

### Unlike Comment (Requires Auth)
```http
POST /api/comments/{id}/unlike
Authorization: Bearer <token>
```

## Users API

### Get Current User Info (Requires Auth)
```http
GET /api/users/me
Authorization: Bearer <token>
```

### Get User by ID
```http
GET /api/users/{id}
```

### Get User's Publications
```http
GET /api/users/{id}/publications
```

### Get User's Comments
```http
GET /api/users/{id}/comments
```

### Update Current User Profile (Requires Auth)
```http
PUT /api/users/me
Content-Type: application/json
Authorization: Bearer <token>

{
  "email": "new-email@example.com"
}
```

## Sections API

### Get All Sections
```http
GET /api/sections
```

### Get Section by ID
```http
GET /api/sections/{id}
```

### Get Section by Name
```http
GET /api/sections/by-name/{sectionName}
```

## 🔍 Search API

### Basic Search
```http
GET /api/search/publications?searchTerm=fishing&page=1&pageSize=10
```

### Get Search Suggestions
```http
GET /api/search/suggestions?term=fish
```

### Advanced Search
```http
POST /api/search/advanced
Content-Type: application/json

{
  "searchTerm": "fishing techniques",
  "page": 1,
  "pageSize": 10
}
```

## Statistics API

### Overview Statistics
```http
GET /api/stats/overview
```

**Response:**
```json
{
  "totalPublications": 150,
  "seaPublications": 75,
  "freshwaterPublications": 60,
  "tacklePublications": 25,
  "actionsPublications": 15,
  "currentYearPublications": 45,
  "currentMonthPublications": 8,
  "otherPublications": 15,
  "lastUpdated": "2024-10-30T10:30:00Z"
}
```

### Section Statistics
```http
GET /api/stats/sections
```

### Trend Statistics
```http
GET /api/stats/trends
```

### Popular Content Statistics
```http
GET /api/stats/popular
```

## Base URLs

- **Development:** `http://localhost:51034`
- **Production:** `https://fishspindays-prod-ne-app.azurewebsites.net`

## Response Format

All API endpoints returns JSON in the following format:

### Success Response
```json
{
  "data": { ... },
  "message": "Success message (optional)"
}
```

### Error Response
```json
{
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"] // optional
}
```

## Security Features

- **JWT Authentication** for authenticated endpoints
- **Input validation** с SafeHTML for XSS protection
- **Rate limiting** and security logging
- **CORS** support for frontend apps
- **Request/Response logging** for monitoring

## Status Codes

- `200 OK` - Successful request
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - Missing or invalid authentication
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Frontend Integration Examples

### JavaScript (Fetch API)
```javascript
// Get publications
const response = await fetch('/api/publications?page=1');
const publications = await response.json();

// Create publication (with auth)
const response = await fetch('/api/publications', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    title: 'My Fishing Adventure',
    description: '<p>Great day at the lake...</p>',
    section: 'Freshwater fishing'
  })
});
```

### React Example
```jsx
const [publications, setPublications] = useState([]);

useEffect(() => {
  const fetchPublications = async () => {
    try {
   const response = await fetch('/api/publications');
  const data = await response.json();
      setPublications(data.publications);
    } catch (error) {
      console.error('Error fetching publications:', error);
    }
  };
  
  fetchPublications();
}, []);
```

## Configuration

To use the APIin production environment:

1. Set JWT settings в `appsettings.json`
2. Configure CORS policies
3. Set logging и monitoring
4. Set rate limiting if needed

## Support

For more details about the API, please contact the development team.