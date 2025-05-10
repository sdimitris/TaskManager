export type Task = {
    title: string;
    description: string;
    status: string;
    createdAt: string;
    assigneeUsername: string | null;
    id: number;
};