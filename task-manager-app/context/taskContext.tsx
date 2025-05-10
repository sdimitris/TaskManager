// context/TaskContext.tsx
import { createContext, useContext, useState, ReactNode } from "react";
import { Task } from "../models/task";

type TaskContextType = {
  selectedTask: Task | null;
  setSelectedTask: (task: Task | null) => void;
};

const TaskContext = createContext<TaskContextType | undefined>(undefined);

export const TaskProvider = ({ children }: { children: ReactNode }) => {
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);

  return (
    <TaskContext.Provider value={{ selectedTask, setSelectedTask }}>
      {children}
    </TaskContext.Provider>
  );
};

export const useTaskContext = () => {
  const context = useContext(TaskContext);
  if (!context) throw new Error("useTaskContext must be used within TaskProvider");
  return context;
};
