import React from 'react';
import { CompanyOrgComponent } from '../frontend/components/CompanyOrgComponent';

export const Manager1Page: React.FC<{ companyId: number }> = ({ companyId }) => {
  return (
    <div className="container mt-4">
      <div className="row mb-4">
        <div className="col-md-8">
          <h1 className="h2">Manager1 - Company Dashboard</h1>
          <p className="text-muted">Company Role - Manager1 Page</p>
        </div>
      </div>

      <div className="row">
        <div className="col-md-8">
          <CompanyOrgComponent companyId={companyId} />
        </div>
      </div>

      <div className="row mt-4">
        <div className="col-md-12">
          <div className="card">
            <div className="card-header">
              <h5>Manager Features</h5>
            </div>
            <div className="card-body">
              <ul className="list-group list-group-flush">
                <li className="list-group-item">User Management</li>
                <li className="list-group-item">Property Management</li>
                <li className="list-group-item">Reporting</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
