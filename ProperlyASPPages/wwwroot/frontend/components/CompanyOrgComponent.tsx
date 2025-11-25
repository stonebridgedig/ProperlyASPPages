import React, { useEffect, useState } from 'react';
import { getManagementOrg } from '../api';

interface ManagementOrgProps {
  managementId: number;
}

export const ManagementOrgComponent: React.FC<ManagementOrgProps> = ({ managementId }) => {
  const [management, setManagement] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getManagementOrg(managementId)
      .then(setManagement)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [managementId]);

  if (loading) return <div className="alert alert-info">Loading...</div>;
  if (error) return <div className="alert alert-danger">Error: {error}</div>;

  return (
    <div className="card">
      <div className="card-body">
        <h5 className="card-title">{management?.Name}</h5>
        <p className="card-text">{management?.Email}</p>
        <small className="text-muted">{management?.Address}, {management?.City}, {management?.State}</small>
      </div>
    </div>
  );
};
