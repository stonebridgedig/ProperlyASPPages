export async function getManagementOrg(id: number) {
  const response = await fetch(`/api/management/${id}`);
  if (!response.ok) throw new Error(`Failed to fetch management: ${response.status}`);
  return response.json();
}

export async function getManagementUser(id: number) {
  const response = await fetch(`/api/management/${id}/users`);
  if (!response.ok) throw new Error(`Failed to fetch management users: ${response.status}`);
  return response.json();
}
