# Architecture: C# Backend + Razor Frontend

## Backend Architecture (Pure C#)

All backend business logic and data access is handled through **ASP.NET Core API Controllers**.

### API Controllers Location
`ProperlyASPPages/Controllers/Api/`

### Current Controllers
- `CompanyController.cs` - Company organization and user management

### API Endpoints Pattern
```
GET    /api/company/{id}
GET    /api/company/{id}/users
POST   /api/company
PUT    /api/company/{id}
DELETE /api/company/{id}
```

## Frontend Architecture (Razor + React)

- **Razor Pages** (.cshtml) - Server-rendered UI and page layout
- **React Components** - Interactive client-side components loaded via CDN importmap
- **Code-behind** (.cshtml.cs) - Page logic for rendering, authentication checks, etc.

### Frontend Structure
```
Pages/
??? Shared/
?   ??? _Layout.cshtml (with React importmap)
??? Company/
?   ??? Manager1.cshtml
?   ??? Manager2.cshtml
??? Owner/
?   ??? Owner1.cshtml
?   ??? Owner2.cshtml
??? Tenant/
?   ??? Tenant1.cshtml
?   ??? Tenant2.cshtml
??? Service/
    ??? Service1.cshtml
    ??? Service2.cshtml

wwwroot/frontend/
??? api.ts (API client)
??? components/
    ??? CompanyOrgComponent.tsx
```

## Data Flow

```
React Component
    ?
fetch('/api/company/{id}')
    ?
CompanyController
    ?
ICompanyRepository
    ?
Dapper + SQL Server
```

## Key Principles

1. **Backend is Pure C#**: Controllers, repositories, services
2. **No business logic in Razor**: Razor is only for page rendering and authentication
3. **React for interactivity**: Client-side state management and UI interactions
4. **API-driven**: React communicates with backend only through API endpoints
5. **Separation of Concerns**: Backend and frontend are independently testable

## Adding New Features

### To add a new API endpoint:
1. Create a controller in `Controllers/Api/`
2. Inject repository and implement action method
3. Call from React via `fetch('/api/...')`

### To add a new page:
1. Create `.cshtml` file in `Pages/`
2. Create `.cshtml.cs` code-behind if needed
3. Add React components in the page's `@section Scripts`
