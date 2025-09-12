import React, { useState } from "react";
import "./AddTask.css";
import { Link, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import { createTask } from "../../apiEndpoints";
import { useTranslation } from "react-i18next";

const AddTask: React.FC = () => {
  const [title, setTitle] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [isUrgent, setIsUrgent] = useState(false);
  const [description, setDescription] = useState("");
  const navigate = useNavigate();
  const { t } = useTranslation('addTask');

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
        const startDateTime = new Date(`${startDate}T${startTime}`);
        const endDateTime = endDate 
        ? new Date(`${endDate}T${endTime}`)
        : new Date(`${startDate}T${startTime}`);

        const userId = localStorage.getItem('userId');
        if (!userId) {
        throw new Error("User ID not found");
        }

        const taskData = {
        Title: title,
        Description: description,
        StartDate: startDateTime.toISOString(),
        EndDate: endDateTime.toISOString(),
        IsUrgent: isUrgent,
        UserId: parseInt(userId, 10),
        };

      const response = await fetch(createTask, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(taskData),
      });

      if (!response.ok) {
        throw new Error("Failed to create task");
      }

      navigate("/tasks");
      
    } catch (error) {
      console.error("Error creating task:", error);
      alert(t('errors.createFailed'));
    }
  };

  return (
    <div className="main-container">
        <Header />
        <div className="wrapper">
            <div className="card">
                <form className="form" onSubmit={handleAdd}>
                    <h2 className="title">{t('title')}</h2>
                    <p className="subtitle">{t('subtitle')}</p>

                    <div className="inputGroup">
                        <span className="numCircle">1</span>
                        <input
                            type="text"
                            placeholder={t('inputs.title')}
                            className="input"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            required
                        />
                    </div>

                    <div className="date-label-container">
                        <label>{t('inputs.startDate')}</label>
                    </div>
                    
                    <div className="inputGroup">
                        <span className="numCircle">2</span>
                        <input
                            type="date"
                            className="input"
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                            required
                        />
                        <input
                            type="time"
                            className="input"
                            value={startTime}
                            onChange={(e) => setStartTime(e.target.value)}
                            required
                        />
                    </div>
                    
                    <div className="date-label-container">
                        <label>{t('inputs.endDate')}</label>
                    </div>

                    <div className="inputGroup">
                        <span className="numCircle">3</span>
                        <input
                            type="date"
                            className="input"
                            value={endDate}
                            onChange={(e) => setEndDate(e.target.value)}
                        />
                        <input
                            type="time"
                            className="input"
                            value={endTime}
                            onChange={(e) => setEndTime(e.target.value)}
                        />
                    </div>

                    <div className="inputGroup">
                        <span className="numCircle">4</span>
                        <label className="checkboxLabel">
                            <input
                                type="checkbox"
                                checked={isUrgent}
                                onChange={(e) => setIsUrgent(e.target.checked)}
                            />
                            <span className="is-urgent">{t('inputs.urgent')}</span>
                        </label>
                    </div>

                    <div className="inputGroup">
                    <span className="numCircle">5</span>
                    <textarea
                        placeholder={t('inputs.description')}
                        className="input add-task-description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        rows={4}
                    />
                    </div>

                    <div className="buttonGroup">
                        <button type="submit" className="addBtn">{t('buttons.newTask')}</button>
                        <Link to="/tasks" style={{ textDecoration: "none" }}>
                            <button type="button" className="cancelBtn">{t('buttons.cancel')}</button>
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    </div>
  );
};

export default AddTask;