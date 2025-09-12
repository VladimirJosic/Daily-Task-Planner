import React from 'react';
import './Week_TaskModal.css';  // Dodaj stilove (napravi sledeći korak)

type Task = {
  id: number;
  title: string;
  description: string;
  isActive: boolean;
  startDate: Date;
  endDate: Date;
};

type Props = {
  task: Task | null;
  onClose: () => void;
};

const TaskModal: React.FC<Props> = ({ task, onClose }) => {
  if (!task) return null;  // Ako nema selektovanog taska, ne prikazuje ništa

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <h2>{task.title}</h2>
        <p><b>Description:</b> {task.description}</p>
        <p><b>Status:</b> {task.isActive ? 'Active' : 'Inactive'}</p>
        <p><b>startDate:</b> {new Date(task.startDate).toLocaleString()}</p>
        <p><b>endDate:</b> {new Date(task.endDate).toLocaleString()}</p>
        <button onClick={onClose}>Close</button>
      </div>
    </div>
  );
};

export default TaskModal;