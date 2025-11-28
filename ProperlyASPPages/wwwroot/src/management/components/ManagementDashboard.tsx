import React from 'react';

interface ManagementData {
    currentUser?: {
        id: number;
        name: string;
        email: string;
        managementOrgId?: number;
    };
    pendingInvitations: number;
    recentInvitations: Array<{
        id: number;
        email: string;
        role: string;
        invitedDate: string;
    }>;
}

interface ManagementDashboardProps {
    data: ManagementData;
}

export const ManagementDashboard: React.FC<ManagementDashboardProps> = ({ data }) => {
    const [invitations] = React.useState(data.recentInvitations);

    return (
        <div className="row">
            <div className="col-md-4 mb-4">
                <div className="card">
                    <div className="card-header">
                        <h5 className="mb-0">
                            <i className="bi bi-envelope-plus me-2"></i>
                            Pending Invitations
                        </h5>
                    </div>
                    <div className="card-body">
                        <h2 className="mb-0">{data.pendingInvitations}</h2>
                        <small className="text-muted">Awaiting response</small>
                    </div>
                </div>
            </div>

            <div className="col-md-8 mb-4">
                <div className="card">
                    <div className="card-header">
                        <h5 className="mb-0">
                            <i className="bi bi-clock-history me-2"></i>
                            Recent Invitations
                        </h5>
                    </div>
                    <div className="card-body">
                        {invitations.length > 0 ? (
                            <div className="table-responsive">
                                <table className="table table-hover">
                                    <thead>
                                        <tr>
                                            <th>Email</th>
                                            <th>Role</th>
                                            <th>Date Invited</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {invitations.map(inv => (
                                            <tr key={inv.id}>
                                                <td>{inv.email}</td>
                                                <td>
                                                    <span className="badge bg-primary">{inv.role}</span>
                                                </td>
                                                <td>{new Date(inv.invitedDate).toLocaleDateString()}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        ) : (
                            <p className="text-muted mb-0">No pending invitations</p>
                        )}
                    </div>
                </div>
            </div>

            <div className="col-12">
                <div className="card">
                    <div className="card-header">
                        <h5 className="mb-0">
                            <i className="bi bi-speedometer2 me-2"></i>
                            Quick Actions
                        </h5>
                    </div>
                    <div className="card-body">
                        <div className="d-grid gap-2 d-md-flex">
                            <button className="btn btn-primary">
                                <i className="bi bi-person-plus me-2"></i>
                                Invite User
                            </button>
                            <button className="btn btn-outline-secondary">
                                <i className="bi bi-building me-2"></i>
                                Manage Properties
                            </button>
                            <button className="btn btn-outline-secondary">
                                <i className="bi bi-file-earmark-text me-2"></i>
                                View Reports
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
