import React from 'react';

type SidebarProps = {
  onSelect: (option: 'all' | 'my') => void;
};

export const Sidebar: React.FC<SidebarProps> = ({ onSelect }) => {
  return (
    <div className="w-64 bg-gray-800 text-white h-screen p-5">
      <h2 className="text-2xl font-bold mb-8">Task Manager</h2>
      <ul>
        <li
          className="cursor-pointer hover:bg-gray-700 p-2 rounded"
          onClick={() => onSelect('all')}
        >
          All Tasks
        </li>
        <li
          className="cursor-pointer hover:bg-gray-700 p-2 rounded mt-4"
          onClick={() => onSelect('my')}
        >
          My Tasks
        </li>
      </ul>
    </div>
  );
};
