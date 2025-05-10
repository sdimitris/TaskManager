import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { Task } from "../../models/task";

export default function TaskDetailPage() {
  const router = useRouter();
  const { id } = router.query;
  const [task, setTask] = useState<Task | null>(null);
  const [users, setUsers] = useState<{ id: string; username: string }[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token || !id) return;

    const fetchTask = async () => {
      try {
        const res = await fetch(`http://localhost:8080/api/Task/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) throw new Error("Failed to fetch task");
        const data = await res.json();
        setTask(data);

        const userRes = await fetch("http://localhost:8080/getUsers", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!userRes.ok) throw new Error("Failed to fetch users");
        const usersData = await userRes.json();
        setUsers(usersData);
      } catch (err: any) {
        setError(err.message || "Failed to load task");
      } finally {
        setLoading(false);
      }
    };

    fetchTask();
  }, [id]);

  const handleSave = async () => {
    const token = localStorage.getItem("token");
    if (!token || !task) return;

    if (task.assigneeUsername === "") {
      task.assigneeUsername = null;
    }

    try {
      const res = await fetch(`http://localhost:8080/api/Task/${task.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(task),
      });

      if (!res.ok) throw new Error("Update failed");

      alert("Task updated successfully!");
    } catch {
      alert("Failed to update task");
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    if (!task) return;
    const { name, value } = e.target;
    setTask({
      ...task,
      [name]: name === "status" ? parseInt(value) : value,
    });
  };

  if (loading) return <p>Loading task...</p>;
  if (error) return <p className="text-red-500">{error}</p>;
  if (!task) return null;

  return (
    <div className="text-black max-w-2xl mx-auto mt-10 p-6 bg-white shadow rounded">
      <h2 className="text-2xl font-bold mb-6">Task Details</h2>

      <label className="block mb-2 font-medium">Title</label>
      <input
        className="w-full border px-3 py-2 mb-4"
        name="title"
        value={task.title}
        onChange={handleChange}
      />

      <label className="block mb-2 font-medium">Description</label>
      <textarea
        className="w-full border px-3 py-2 mb-4"
        name="description"
        value={task.description}
        onChange={handleChange}
      />

      <label className="block mb-2 font-medium">Assignee</label>
      <select
        name="assigneeUsername"
        className="w-full border px-3 py-2 mb-4"
        value={task.assigneeUsername || ""}
        onChange={handleChange}
      >
        <option value="">Select Assignee (Optional)</option>
        {users.map((user) => (
          <option key={user.id} value={user.username}>
            {user.username}
          </option>
        ))}
      </select>

      <label className="block mb-2 font-medium">Status</label>
      <select
        name="status"
        className="w-full border px-3 py-2 mb-4"
        value={task.status}
        onChange={handleChange}
      >
        <option value={1}>To do</option>
        <option value={2}>In Progress</option>
        <option value={3}>Done</option>
      </select>

      <label className="block mb-2 font-medium">Created At</label>
      <div className="w-full border px-3 py-2 mb-6">{task.createdAt}</div>

      <div className="flex gap-4 mt-4">
        <button
          onClick={handleSave}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Save Changes
        </button>

        <button
          onClick={() => router.push("/home")}
          className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
        >
          Home Page
        </button>
      </div>
    </div>
  );
}
