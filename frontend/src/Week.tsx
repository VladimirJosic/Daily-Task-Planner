import React, { useEffect, useState } from 'react';
import './Week.css';
import theLogo from './assets/logo.png';
import TaskModal from './Week_TaskModal';
import { Link } from 'react-router-dom';
import { useAuth } from './pages/auth/AuthContext';

const days = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"] as const;
type Day = typeof days[number];

type Task = {
  id: number;
  title: string;
  description: string;
  isActive: boolean;
  startDate: Date;
  endDate: Date;
  isUrgent: boolean;
};

type TasksPerDay = {
  [key in Day]: Task[];
};

const Week: React.FC = () => {
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);

  const openTaskDetails = (task: Task) => {
    setSelectedTask(task);
  };

  const closeTaskDetails = () => {
    setSelectedTask(null);
  };


  const [tasksPerDay, setTasksPerDay] = useState<TasksPerDay>({
    MON: [],
    TUE: [],
    WED: [],
    THU: [],
    FRI: [],
    SAT: [],
    SUN: [],
  });

  const tasksApiUrl = import.meta.env.VITE_API_DAILY_TASK_URL || "/api/DailyTask";
  const { accessToken } = useAuth();

  useEffect(() => {
    if (!accessToken) {
      console.error("Nema access tokena!");
      return;
    }

    const fetchUserAndTasks = async () => {
      try {
        // Izračunaj datum range
        const startDate = new Date(); // danas
        const endDate = new Date();
        endDate.setDate(startDate.getDate() + 6);

        const startStr = startDate.toISOString();
        const endStr = endDate.toISOString();

        // Dohvati taskove (backend uzima userId iz JWT tokena)
        const tasksRes = await fetch(
          `${tasksApiUrl}/get-all-date-range?startDate=${startStr}&endDate=${endStr}`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );

        if (!tasksRes.ok) throw new Error("Neuspešno dohvatanje taskova");

        const data: Task[] = await tasksRes.json();
        // 4. Grupisanje po danima
        const organized: TasksPerDay = {
          MON: [],
          TUE: [],
          WED: [],
          THU: [],
          FRI: [],
          SAT: [],
          SUN: [],
        };
        
        data.forEach((task) => {
          const taskDate = new Date(task.startDate);
          console.log(task.startDate)
          const day = taskDate.toLocaleDateString("en-US", { weekday: "short" }).toUpperCase();
          console.log(day)
          const normalized = day.slice(0, 3); // "MON", "TUE" itd.
          if (days.includes(normalized as Day)) {
            organized[normalized as Day].push(task);
          }
        });

        setTasksPerDay(organized);
      } catch (err) {
        console.error("Greška:", err);
      }
    };

    fetchUserAndTasks();
  }, [tasksApiUrl, accessToken]);

  return (
    <div className="weekly-wrapper">
      <div className="weekly-header">
        <img src={theLogo} alt="Logo" className="logo" />
        <div className="weekly-title">WEEKLY TASKS</div>
        <Link to="/tasks" className="back-link">← Back to Tasks</Link>
      </div>

      <div className="weekly-days-row">
        {days.map((day) => (
          <div key={day} className="weekly-day">{day}</div>
        ))}
      </div>

      <div className="weekly-tasks-row">
        {days.map((day) => (
          <div key={day} className="tasks-col">
            {tasksPerDay[day].length === 0 && <div className="task-empty"></div>}
            {tasksPerDay[day].map((task) => (
              <div
               key={task.id}
               className={`task-card ${task.isUrgent ? 'checked' : ''}`}
               onClick={() => openTaskDetails(task)}    // <-- dodaj klik handler
               style={{ cursor: 'pointer' }}             // <-- da pokazuje da je klikabilno
               >
                <span>{task.title}</span>
                <input type="checkbox" checked={task.isUrgent} readOnly />
              </div>
            ))}
          </div>
        ))}
      </div>
      <TaskModal task={selectedTask} onClose={closeTaskDetails} />
    </div>
  );
};

export default Week;
