# API Controllers Documentation

All backend business logic is handled through ASP.NET Core API Controllers. No Razor for backend.

## Available API Endpoints

### Company Management
**Base URL:** `/api/company`

```
GET    /api/company/{id}              - Get company by ID
GET    /api/company/{id}/users        - Get company users
POST   /api/company                   - Create company
```

### Company Invitations
**Base URL:** `/api/invitations`

```
GET    /api/invitations/{id}          - Get invitation by ID
GET    /api/invitations/token/{token} - Get invitation by token
GET    /api/invitations/company/{companyId} - Get pending invitations
POST   /api/invitations               - Create invitation
PUT    /api/invitations/{id}/status   - Update invitation status
```

### System Administration
**Base URL:** `/api/systemadmin`

```
GET    /api/systemadmin/{id}          - Get system admin by ID
GET    /api/systemadmin               - Get all system admins
GET    /api/systemadmin/identity/{identityUserId} - Get by identity user ID
GET    /api/systemadmin/email/{email} - Get by email
GET    /api/systemadmin/{id}/check    - Check if user is system admin
POST   /api/systemadmin               - Create system admin
PUT    /api/systemadmin/{id}          - Update system admin
DELETE /api/systemadmin/{id}          - Delete system admin
GET    /api/systemadmin/activity-logs/{adminId} - Get activity logs
GET    /api/systemadmin/activity-logs/recent - Get recent activity
POST   /api/systemadmin/{adminId}/activity-logs - Log activity
```

## Controller Files

| Controller | Path | Responsibility |
|-----------|------|-----------------|
| `CompanyController` | `Controllers/Api/CompanyController.cs` | Company CRUD and user management |
| `InvitationsController` | `Controllers/Api/InvitationsController.cs` | Invitation workflow management |
| `SystemAdminController` | `Controllers/Api/SystemAdminController.cs` | Admin operations and activity logging |

## Response Format

All endpoints return JSON responses:

### Success Response (2xx)
```json
{
  "id": 1,
  "name": "Example",
  ...
}
```

### Error Response (4xx/5xx)
```json
{
  "error": "Error message here"
}
```

### Created Response (201)
```json
{
  "id": 1
}
```

## Authentication

Controllers can be decorated with `[Authorize]` when needed to require authentication.

Example:
```csharp
[Authorize]
[HttpGet("{id:int}")]
public async Task<IActionResult> GetCompanyOrg(int id)
{
    // ...
}
```

## Adding New Controllers

1. Create a new controller class in `Controllers/Api/`
2. Inherit from `ControllerBase`
3. Add `[ApiController]` and `[Route("api/[controller]")]` attributes
4. Inject repositories via constructor
5. Implement action methods with appropriate HTTP verbs

Example:
```csharp
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyRepository _repo;
    
    public PropertiesController(IPropertyRepository repo)
    {
        _repo = repo;
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProperty(int id)
    {
        var property = await _repo.GetPropertyByIdAsync(id);
        if (property == null)
            return NotFound();
        return Ok(property);
    }
}
```
