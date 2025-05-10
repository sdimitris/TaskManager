import { ReactElement, useEffect, useState } from "react";
import { useRouter } from "next/router";
import { Task } from "../models/task";
import { fetchTasks } from "../api/task"; // Import the fetch functions
import { Sidebar } from "@/components/sidebar";
import Link from "next/link";
import { useTaskContext } from "@/context/taskContext";

export default function HomePage() {
  const router = useRouter();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<"all" | "my">("all"); // Track the selected view
  const { setSelectedTask } = useTaskContext();

  // Fetch tasks based on the selected view
  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      router.replace("/login");
      return;
    }

    const fetchData = async () => {
      try {
        const allTasks: Task[] = await fetchTasks(token); // Fetch all tasks

        if (view === "all") {
          // Show all tasks if 'all' view is selected
          setTasks(allTasks);
        } else {
          // Filter tasks by assignee username if 'my' view is selected
          const username = getUsernameFromToken(token); // Extract username from token
          const myTasks: Task[] = allTasks.filter(
            (task) => task.assigneeUsername === username
          );
          setTasks(myTasks);
        }
      } catch (err: any) {
        setError(err.message); // Handle error
      } finally {
        setLoading(false); // End loading
      }
    };

    fetchData();
  }, [view]); // Fetch tasks whenever the view changes

  const getUsernameFromToken = (token: string) => {
    const decoded = JSON.parse(atob(token.split(".")[1])); // Decode JWT token
    return decoded.username; // Assuming the token has a 'username' field
  };

  const mapToStatus =  (status: number) => {
    switch (status) {
      case 1:
        return "To do";
      case 2:
        return "In Progress";
      case 3:
        return "Done";
    }
  };
  // Logout function to clear the token and redirect
  const handleLogout = () => {
    localStorage.removeItem("token"); // Remove token from localStorage
    router.replace("/login"); // Redirect to the login page
  };

  return (
    <div className="flex relative">
      <Sidebar onSelect={setView} /> {/* Pass setView to Sidebar */}
      <div className="text-black p-8 flex-1">
        {/* Logout button */}
        <button
          onClick={handleLogout}
          className="absolute top-4 right-4 bg-red-500 text-white py-2 px-4 rounded-md hover:bg-red-600"
        >
          Logout
        </button>

        <h1 className="text-3xl font-bold mb-6">Welcome</h1>

        {loading && <p>Loading tasks...</p>}
        {error && <p className="text-red-500">{error}</p>}

        {tasks.length > 0 ? (
          <div className="text-black overflow-x-auto">
            <table className="min-w-full bg-white shadow-lg rounded-lg">
              <thead>
                <tr className="bg-gray-100 text-left">
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">
                    Title
                  </th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">
                    Description
                  </th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">
                    Assignee
                  </th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">
                     Creation Time
                  </th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600">
                    Status
                  </th>
                  <th className="px-6 py-3 text-sm font-medium text-gray-600"></th>
                </tr>
              </thead>
              <tbody>
                {tasks.map((task, index) => (
                  <tr key={`${task.id}`} className="border-b">
                    <td className="px-6 py-4 text-sm text-gray-800">
                      {task.title}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-800">
                      {task.description}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-800">
                      {task.assigneeUsername}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-800">
                      {task.createdAt}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-800">
                      {mapToStatus(task.status)}
                    </td>

                    <Link
                      href={`/tasks/${task.id}`}
                      onClick={(e) => {
                        e.preventDefault();
                        setSelectedTask(task);
                        router.push(`/tasks/${task.id}`);
                      }}
                    >
                      Show Task
                    </Link>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p>No tasks available.</p>
        )}
        <div className="flex gap-4 mt-4">
          <button
            onClick={() => router.push("tasks/create")}
            className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
          >
            Create Task
          </button>
        </div>
      </div>
    </div>
  );
}
