import React from 'react';
import { createRoot } from 'react-dom/client';
import { ManagementDashboard } from './components/ManagementDashboard';

const container = document.getElementById('management-dashboard-root');
if (container) {
    const root = createRoot(container);
    const dataElement = document.getElementById('management-data');
    const data = dataElement ? JSON.parse(dataElement.textContent || '{}') : {};
    
    root.render(
        <React.StrictMode>
            <ManagementDashboard data={data} />
        </React.StrictMode>
    );
}
