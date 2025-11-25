# React Frontend - CDN Based

This project uses React loaded from CDN via ES modules importmap. **No npm or build tools required.**

## How It Works

1. **_Layout.cshtml** defines an importmap that maps module names to CDN URLs
2. **Razor Pages** contain `<script type="module">` blocks that import React, React DOM, etc.
3. React components render into a `<div id="react-root"></div>` container

## Structure

```
wwwroot/frontend/
??? api.ts                 - API utility functions
??? components/
    ??? CompanyOrgComponent.tsx - Reusable React components
```

## Creating a React Page

1. Add a `<div id="react-root"></div>` to your Razor page
2. Add a `@section Scripts` block with a module script:

```html
<div id="react-root"></div>

@section Scripts {
    <script type="module">
        import React from 'react';
        import { createRoot } from 'react-dom';

        const MyComponent = () => {
            return React.createElement('div', null, 'Hello from React!');
        };

        const container = document.getElementById('react-root');
        if (container) {
            const root = createRoot(container);
            root.render(React.createElement(MyComponent));
        }
    </script>
}
```

## Available Modules (via importmap)

- `react` - React library
- `react-dom` - React DOM renderer
- `react-router` - React Router for navigation

## API Endpoints

- `/api/company/{id}` - Get company data as JSON

## Development & Production

The same code works in both environments:
- **Development**: Just run your ASP.NET Core app
- **Production**: Just deploy your ASP.NET Core app (no build step needed)
