export const fetchTasks = async (token: string) => {
  const response = await fetch('http://localhost:8080/api/Task/', {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch tasks');
  }

  return response.json();
};