import { ReactElement, useEffect, useState } from 'react';
import { useRouter } from 'next/router';
import { Task } from '../models/task';
import { fetchTasks } from '../api/task'; // Import the fetch functions
import { Sidebar } from '@/components/sidebar';

export default function HomePage() {
  const router = useRouter();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<'all' | 'my'>('all'); 
  const [username,setUsername] = useState<string>();

  useEffect(() => {
    const token = localStorage.getItem('token');

    if (!token) {
      router.replace('/');
      return;
    }

    const username = getUsernameFromToken(token); 
    setUsername(username);
    const fetchData = async () => {
      try {
        setLoading(true); // Start loading
        const allTasks: Task[] = await fetchTasks(token); // Fetch all tasks

        if (view === 'all') {
          setTasks(allTasks);
        } else {
          const myTasks = allTasks.filter((task) => task.assigneeUsername === username);
          setTasks(myTasks);
        }
      } catch (err: any) {
        setError(err.message); 
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [view]); // Fetch tasks whenever the view changes

  const getUsernameFromToken = (token: string) => {
    const decoded = JSON.parse(atob(token.split('.')[1])); // Decode JWT token
    return decoded.username; // Return the username from the token
  };

  return (
    <div className="flex">
      <Sidebar onSelect={setView} />
      <div className="text-black p-8 flex-1">
        <h1 className="text-3xl font-bold mb-6">Welcome {username}</h1>

        {loading && <p>Loading tasks...</p>}
        {error && <p className="text-red-500">{error}</p>}

        {tasks.length > 0 ? (
          <div className="text-black overflow-x-auto">
            <table className="min-w-full bg-white shadow-lg rounded-lg">
              <thead>
                <tr className="bg-gray-100 text-left">
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">Title</th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">Description</th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">Assignee</th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">Creation Time</th>
                </tr>
              </thead>
              <tbody>
                {tasks.map((task) => (
                 <tr key={`${task.title}-${task.createdAt}`} className="border-b">
                    <td className="px-6 py-4 text-sm text-gray-800">{task.title}</td>
                    <td className="px-6 py-4 text-sm text-gray-800">{task.description}</td>
                    <td className="px-6 py-4 text-sm text-gray-800">{task.assigneeUsername}</td>
                    <td className="px-6 py-4 text-sm text-gray-800">{task.createdAt}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p>No tasks available.</p>
        )}
      </div>
    </div>
  );
}