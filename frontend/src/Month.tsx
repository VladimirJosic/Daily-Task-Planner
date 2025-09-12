import React, { useState, useEffect } from "react";
import "./Month.css";
import { Link } from 'react-router-dom';
import { getMe } from './apiEndpoints';

const daysShort = ["S", "M", "T", "W", "T", "F", "S"];
const months = [
  "January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
];

function getDaysInMonth(year: number, month: number): number {
  return new Date(year, month + 1, 0).getDate();
}

type Task = {
  title: string;
  isActive: boolean;
  startDate?: string; 
  isUrgent: boolean;
};

type MonthlyTasks = {
  [day: number]: Task[];
};

const Month: React.FC = () => {
  const today = new Date();
  const [currentMonth, setCurrentMonth] = useState<number>(today.getMonth());
  const [currentYear, setCurrentYear] = useState<number>(today.getFullYear());
  const [selectedDate, setSelectedDate] = useState<Date>(today);
  const [hoveredDay, setHoveredDay] = useState<number | null>(null);
  const [showSidePanel, setShowSidePanel] = useState<boolean>(false);
  const [sidePanelDay, setSidePanelDay] = useState<number | null>(null);
  const [monthlyTasks, setMonthlyTasks] = useState<MonthlyTasks>({});

  const daysInMonth = getDaysInMonth(currentYear, currentMonth);
  const firstDay = new Date(currentYear, currentMonth, 1).getDay();
  const tasksApiUrl = import.meta.env.VITE_API_DAILY_TASK_URL || "http://localhost:3000/api/DailyTask";
   const accessToken = localStorage.getItem("accessToken");
  
  useEffect(() => {
    const fetchMonthlyTasks = async () => {
      try {
         // 1. Dohvati trenutno ulogovanog korisnika
                const userRes = await fetch(`${getMe}`, {
                  headers: {
                    Authorization: `Bearer ${accessToken}`,
                  },
                });
        
                if (!userRes.ok) throw new Error("Neuspešan /me zahtev");
        
                const user = await userRes.json();
                const userId = user.id;

        const startDate = `${currentYear}-${(currentMonth + 1).toString().padStart(2, "0")}-01`;
        const endDate = `${currentYear}-${(currentMonth + 1).toString().padStart(2, "0")}-${getDaysInMonth(currentYear, currentMonth)}`;
        
        const tasksRes = await fetch(
          `${tasksApiUrl}/get-all-date-range/${userId}?startDate=${startDate}&endDate=${endDate}`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );

        if (!tasksRes.ok) throw new Error("Neuspešno dohvatanje taskova");

        const data = await tasksRes.json();

        const grouped: MonthlyTasks = {};
        for (const task of data) {
          const day = new Date(task.startDate).getDate();
          if (!grouped[day]) grouped[day] = [];
          grouped[day].push({
            title: task.title,
            isActive: task.isActive,
            isUrgent: task.isUrgent,
          });
        }

        setMonthlyTasks(grouped);
      } catch (error) {
        console.error("Došlo je do greške:", error);
      }
    };

    fetchMonthlyTasks();
  }, [currentMonth, currentYear]);

  const prevMonth = () => {
    if (currentMonth === 0) {
      setCurrentMonth(11);
      setCurrentYear((y) => y - 1);
    } else {
      setCurrentMonth((m) => m - 1);
    }
    setShowSidePanel(false);
  };

  const nextMonth = () => {
    if (currentMonth === 11) {
      setCurrentMonth(0);
      setCurrentYear((y) => y + 1);
    } else {
      setCurrentMonth((m) => m + 1);
    }
    setShowSidePanel(false);
  };

  function formatDate(date: Date): string {
    return `${months[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;
  }

  function renderTasks(tasks: Task[]) {
    if (!tasks || tasks.length === 0) return <div>No tasks</div>;
    return (
      <ol className="calendar-popup-list">
        {tasks.map((task, index) => (
          <li key={index}>
            <span>{task.title}</span>
            <input
              className="task-popup-checkbox"
              type="checkbox"
              checked={task.isUrgent}
              readOnly
            />
          </li>
        ))}
      </ol>
    );
  }

  function renderTasksForDay(dayNum: number | null) {
    if (!dayNum) return null;
    const tasks = monthlyTasks[dayNum] || [];
    return (
      <div className="calendar-sidepanel">
        <div className="calendar-popup-title">
          Tasks for {months[currentMonth]} {dayNum}, {currentYear}
        </div>
        {renderTasks(tasks)}
        <button className="close-panel-btn" onClick={() => setShowSidePanel(false)}>Close</button>
      </div>
    );
  }

  return (
    <div className="monthly-container">
      <div className="top-bar">
        <div className="top-bar-right">
          <div className="back-link">
            <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
              <path d="M11 4L6 9L11 14" stroke="#9D8900" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
            <Link to="/tasks" className="back-link">Go back to tasks</Link>
          </div>
          <Link to="/user">
            <span className="user-avatar">
              <svg height="32" viewBox="0 0 24 24" width="32" fill="#a472d8">
                <circle cx="12" cy="8" r="4" />
                <path d="M12 14c-4 0-8 2-8 4v2h16v-2c0-2-4-4-8-4z" />
              </svg>
            </span>
          </Link>
        </div>
      </div>

      <div className={`calendar-section ${showSidePanel ? "with-panel" : ""}`}>
        <div className="calendar-card">
          <div className="calendar-selected-date">{formatDate(selectedDate)}</div>
          <hr />
          <div className="calendar-header">
            <button className="calendar-nav" onClick={prevMonth}>&#60;</button>
            <span className="calendar-title">{months[currentMonth]} {currentYear}</span>
            <button className="calendar-nav" onClick={nextMonth}>&#62;</button>
          </div>
          <div className="calendar-days-short">
            {daysShort.map((d, i) => (
              <div key={i} className="calendar-day-short">{d}</div>
            ))}
          </div>
          <div className="calendar-days-grid">
            {[...Array(firstDay)].map((_, i) => (
              <div key={"empty-"+i} className="calendar-day empty"></div>
            ))}
            {Array.from({ length: daysInMonth }, (_, day) => {
              const dayNum = day + 1;
              const isToday =
                dayNum === today.getDate() &&
                currentMonth === today.getMonth() &&
                currentYear === today.getFullYear();
              const isSelected =
                dayNum === selectedDate.getDate() &&
                currentMonth === selectedDate.getMonth() &&
                currentYear === selectedDate.getFullYear();
              const tasks = monthlyTasks[dayNum] || [];

              return (
                <div
                  key={dayNum}
                  className={`calendar-day${isToday ? " today" : ""}${isSelected ? " selected" : ""}`}
                  onClick={() => {
                    setSelectedDate(new Date(currentYear, currentMonth, dayNum));
                    setShowSidePanel(true);
                    setSidePanelDay(dayNum);
                  }}
                  onMouseEnter={() => setHoveredDay(dayNum)}
                  onMouseLeave={() => setHoveredDay(null)}
                  style={{ position: "relative" }}
                >
                  {dayNum}
                  {hoveredDay === dayNum && !showSidePanel && tasks.length > 0 && (
                    <div className="calendar-popup">
                      <div className="calendar-popup-title">Daily tasks</div>
                      {renderTasks(tasks)}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
        {showSidePanel && renderTasksForDay(sidePanelDay)}
      </div>
    </div>
  );
};

export default Month;
