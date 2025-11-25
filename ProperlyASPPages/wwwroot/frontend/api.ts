export async function getCompanyOrg(id: number) {
  const response = await fetch(`/api/company/${id}`);
  if (!response.ok) throw new Error(`Failed to fetch company: ${response.status}`);
  return response.json();
}

export async function getCompanyUser(id: number) {
  const response = await fetch(`/api/company-user/${id}`);
  if (!response.ok) throw new Error(`Failed to fetch company user: ${response.status}`);
  return response.json();
}
