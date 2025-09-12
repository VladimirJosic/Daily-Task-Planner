export interface Task {
  id: number;
  title: string;
  startDate: Date;
  endDate: Date;
  time: string;
  isUrgent: string;
  description: string;
  isActive: boolean;
}