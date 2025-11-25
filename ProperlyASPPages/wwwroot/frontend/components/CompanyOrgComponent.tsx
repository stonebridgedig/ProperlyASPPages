import React, { useEffect, useState } from 'react';
import { getCompanyOrg } from './api';

interface CompanyOrgProps {
  companyId: number;
}

export const CompanyOrgComponent: React.FC<CompanyOrgProps> = ({ companyId }) => {
  const [company, setCompany] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getCompanyOrg(companyId)
      .then(setCompany)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [companyId]);

  if (loading) return <div className="alert alert-info">Loading...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="card">
      <div className="card-body">
        <h5 className="card-title">{company?.Name}</h5>
        <p className="card-text">{company?.Email}</p>
        <small className="text-muted">{company?.Address}, {company?.City}, {company?.State}</small>
      </div>
    </div>
  );
};
