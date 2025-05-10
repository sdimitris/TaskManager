import { useState, useEffect } from "react";
import { useRouter } from "next/router";

export default function CreateTaskPage() {
  const router = useRouter();
  const [task, setTask] = useState({
    title: "",
    description: "",
    assigneeUsername: null as string | null, // Initialize as null
  });
  const [users, setUsers] = useState<{ id: string; username: string }[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Fetch the list of users when the component mounts
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const res = await fetch("http://localhost:8080/getUsers"); // Update with correct endpoint
        if (!res.ok) throw new Error("Failed to fetch users");
        const data = await res.json();
        setUsers(data);
      } catch (err) {
        setError("Failed to load users");
      }
    };

    fetchUsers();
  }, []);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setTask((prev) => ({
      ...prev,
      [name]: value === "" ? null : value, // Set to null if empty string
    }));
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    const token = localStorage.getItem("token");

    if (!token) {
      setError("You must be logged in to create a task.");
      return;
    }

    try {
      const res = await fetch("http://localhost:8080/api/Task", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(task),
      });

      if (!res.ok) {
        throw new Error("Failed to create task");
      }

      alert("Task created successfully!");
      router.push("/home"); // Redirect to home page after creation
    } catch (err: any) {
      setError(err.message || "An error occurred while creating the task.");
    }
  };

  return (
    <div className="text-black max-w-2xl mx-auto mt-10 p-6 bg-white shadow rounded">
      <h2 className="text-2xl font-bold mb-6">Create New Task</h2>

      {error && <p className="text-red-500 mb-4">{error}</p>}

      <form onSubmit={handleCreate}>
        <label className="block mb-2 font-medium">Title</label>
        <input
          type="text"
          name="title"
          className="w-full border px-3 py-2 mb-4"
          value={task.title}
          onChange={handleChange}
          required
        />

        <label className="block mb-2 font-medium">Description</label>
        <textarea
          name="description"
          className="w-full border px-3 py-2 mb-4"
          value={task.description}
          onChange={handleChange}
          required
        />

        <label className="block mb-2 font-medium">Assignee (Optional)</label>
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

        <div className="flex gap-4 mt-4">
          <button
            type="submit"
            className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
          >
            Create Task
          </button>

          <button
            type="button"
            onClick={() => router.push("/home")}
            className="bg-gray-600 text-white px-6 py-2 rounded hover:bg-gray-700"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
