# GitHub Copilot Instructions

## General Guidelines

- **Do NOT create `.md` (Markdown) files** unless explicitly requested by the user
- Focus on code files: `.cs`, `.cshtml`, `.tsx`, `.ts`, `.json`, `.css`, etc.
- When documenting changes, provide explanations in comments within code files or in conversation responses
- If documentation is needed, suggest adding it as XML documentation comments in C# files or JSDoc comments in TypeScript files

## Project-Specific Context

This is a multi-tenant property management web application built on .NET 10 Razor Pages with an integrated React frontend.

### Technology Stack
- **Backend**: ASP.NET Core Razor Pages (.NET 10)
- **Frontend**: React 19.2.0 with TypeScript
- **Styling**: Tailwind CSS
- **Routing**: React Router (HashRouter)
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity with Entity Framework Core
- **Data Access**: Dapper for business data, Entity Framework Core for user identity only

### Architecture
- **Multi-tenant**: Supports multiple property management organizations
- **User Identity Management**: Entity Framework Core
- **Business Data Access**: Dapper ORM for improved performance and flexibility

### Code Style Preferences
- Follow existing coding conventions in the codebase
- Use minimal comments unless necessary for complex logic
- Prefer `var` for local variables in C#
- Use meaningful variable and method names
- Keep methods focused and single-purpose

### File Structure
- Razor Pages: `ProperlyASPPages/Pages/`
- React Components: `ProperlyASPPages/wwwroot/frontend/`
- Static Assets: `ProperlyASPPages/wwwroot/`
- Data Context: `ProperlyASPPages/Data/`
- Services: `ProperlyASPPages/Services/`

## What to Avoid
- Creating README files
- Creating documentation files
- Creating changelog files
- Creating markdown files of any kind
- Over-commenting code

## Preferred Documentation Methods
1. XML documentation comments for public C# APIs
2. JSDoc comments for TypeScript functions and components
3. Inline comments only for complex or non-obvious logic
4. Conversation responses for explanations and summaries
