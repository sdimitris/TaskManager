export type Task = {
    title: string;
    description: string;
    status: number;
    createdAt: string;
    assigneeUsername: string | null;
    id: number;
};