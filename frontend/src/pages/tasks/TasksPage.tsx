import React, { useEffect, useState } from 'react';
import './TasksPage.css';
import AIChat from '../../components/AIChat';
import { Link } from 'react-router-dom';
import Header from '../../components/Header/Header';
import type { Task } from './dtos/task';
import { getAllTasks, deactivateTask, shareTask, getSharedTasks, getAllUsers, searchTasks } from '../../apiEndpoints';
import DateFormatter from '../../components/DateFormatter';
import type { SharedTask, User } from './dtos';
import { FiX } from 'react-icons/fi';


const TasksPage: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedTaskIds, setSelectedTaskIds] = useState<number[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [showSharePopup, setShowSharePopup] = useState(false);
  const [sharedTasks, setSharedTasks] = useState<SharedTask[]>([]);
  const [friends, setFriends] = useState<User[]>([]);
  const [shareLoading, setShareLoading] = useState(false);
  const [shareError, setShareError] = useState<string | null>(null);
  const [shareSuccess, setShareSuccess] = useState<string | null>(null);
  const [shareMode, setShareMode] = useState<'view' | 'share'>('view');
  const [selectedFriendId, setSelectedFriendId] = useState<number | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const userId = localStorage.getItem('userId');
        if (!userId) {
          throw new Error('User ID not found in local storage');
        }

        // Fetch tasks
        const tasksResponse = await fetch(`${getAllTasks}/${userId}`);
        if (!tasksResponse.ok) throw new Error('Failed to fetch tasks');
        const tasksData = await tasksResponse.json();
        setTasks(tasksData.filter((task: Task) => task.isActive !== false));

        // Fetch all users
        const usersResponse = await fetch(getAllUsers);
        if (!usersResponse.ok) throw new Error('Failed to fetch users');
        const usersData: User[] = await usersResponse.json();

        const currentUserId = parseInt(userId);
        const filteredUsers = usersData.filter(user => user.id !== currentUserId);
        setFriends(filteredUsers);

      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unknown error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleCheckboxChange = (taskId: number, isChecked: boolean) => {
    setSelectedTaskIds(prev => 
      isChecked ? [...prev, taskId] : prev.filter(id => id !== taskId)
    );
  };

  const handleSearch = async () => {
    const userId = localStorage.getItem('userId');
    if (!userId) return;

    try {
      setLoading(true);
    
      const params: Record<string, string> = {
        userId: userId.toString()
      };

      if (searchTerm) {
        params.inputQuery = searchTerm;
      }

      const queryString = new URLSearchParams(params).toString();
      
      const response = await fetch(`${searchTasks}?${queryString}`);

      if (!response.ok) {
        throw new Error(response.status === 404 
          ? 'No tasks found matching your criteria' 
          : 'Search failed. Please try again.');
      }

      const data = await response.json();
      setTasks(data.filter((task: Task) => task.isActive !== false));
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const deleteTasks = async () => {
    if (selectedTaskIds.length === 0) return;

    try {
      for (const taskId of selectedTaskIds) {
        const response = await fetch(`${deactivateTask}/${taskId}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
        });
        if (!response.ok) throw new Error(`Failed to delete task ${taskId}`);
      }

      const userId = localStorage.getItem('userId');
      const response = await fetch(`${getAllTasks}/${userId}`);
      const data = await response.json();
      setTasks(data.filter((task: Task) => task.isActive !== false));
      setSelectedTaskIds([]);
    } catch (error) {
      console.error('Error deleting tasks:', error);
      alert('Error deleting tasks. Please try again.');
    }
  };

  const handleShareClick = async () => {
    setShowSharePopup(true);
    setShareMode('view');
    setShareLoading(true);
    setShareError(null);
    
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) throw new Error('User ID not found');
      
      const getSharedTasksResponse = await fetch(`${getSharedTasks}?userId=${userId}`);
      if (!getSharedTasksResponse.ok) throw new Error('Failed to fetch shared tasks');
      
      const sharedTasks = await getSharedTasksResponse.json();
      setSharedTasks(sharedTasks.tasks || []);
    } catch (err) {
      setShareError(err instanceof Error ? err.message : 'Failed to load shared tasks');
    } finally {
      setShareLoading(false);
    }
  };

  const handleShareTask = async () => {
    if (!selectedFriendId || selectedTaskIds.length === 0) {
      setShareError('Please select a friend and at least one task');
      return;
    }

    setShareLoading(true);
    setShareError(null);
    setShareSuccess(null);

    try {
      const userId = localStorage.getItem('userId');
      if (!userId) throw new Error('User ID not found');

      const response = await fetch(`${shareTask}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          UserId: parseInt(userId),
          FriendId: selectedFriendId,
          TaskIds: selectedTaskIds
        })
      });

      const responseText = await response.text();
      let errorMessage = 'Failed to share tasks';

      if (!response.ok) {
        try {
          const errorData = JSON.parse(responseText);
          errorMessage = errorData.message || errorMessage;
        } catch {
          // Handle non-JSON error responses
          if (response.status === 400 && responseText.includes('already shared')) {
            errorMessage = 'You have already shared this task with this user';
          } else if (response.status === 404 && responseText.includes('not found')) {
            errorMessage = responseText;
          }
        }
        throw new Error(errorMessage);
      }

      setShareSuccess('Tasks shared successfully!');
      setSelectedTaskIds([]);
      setSelectedFriendId(null);
      
      // Refresh shared tasks list
      const sharedResponse = await fetch(`${getSharedTasks}?userId=${userId}`);
      if (sharedResponse.ok) {
        const sharedData = await sharedResponse.json();
        setSharedTasks(sharedData.tasks || []);
      }
    } catch (err) {
      setShareError(
        err instanceof Error ? 
        err.message : 
        'An unexpected error occurred while sharing tasks'
      );
    } finally {
      setShareLoading(false);
    }
  };

  const closeSharePopup = () => {
    setShowSharePopup(false);
    setSharedTasks([]);
    setShareError(null);
    setShareSuccess(null);
    setShareMode('view');
  };

  return (
    <div className="main-container">
      <Header />

      {loading ? (
        <div className="content-loading">Loading tasks...</div>
      ) : error ? (
        <div className="content-error">Error: {error}</div>
      ) : (
        <>
          <div className="controls">
            <Link to="/add-task" style={{ textDecoration: 'none' }}>
              <button className="new-task-btn">New task</button>
            </Link>

            <div className="search-bar">
              <button className="menu-icon" onClick={() => setSearchTerm("")}><FiX /></button>
              <input
                type="text"
                placeholder="Search tasks"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') handleSearch();
                }}
              />
              <button className="search-icon" onClick={handleSearch}>🔍</button>
            </div>

            <div className="action-icons">
              <button className='share' onClick={handleShareClick} title="Share task">🔗</button> 
              <button className='delete' onClick={deleteTasks} title="Delete task">🗑️</button>
            </div>
          </div>

          {showSharePopup && (
            <div className="popup-overlay">
              <div className="share-popup">
                <div className="popup-header">
                  <h2>{shareMode === 'view' ? 'Shared Tasks' : 'Share Tasks'}</h2>
                  <button className="close-popup" onClick={closeSharePopup}>
                    &times;
                  </button>
                </div>

                <div className="share-mode-toggle">
                  <button
                    className={`mode-btn ${shareMode === 'view' ? 'active' : ''}`}
                    onClick={() => setShareMode('view')}
                  >
                    View Shared
                  </button>
                  <button
                    className={`mode-btn ${shareMode === 'share' ? 'active' : ''}`}
                    onClick={() => setShareMode('share')}
                  >
                    Share Tasks
                  </button>
                </div>

                {shareLoading ? (
                  <div className="popup-loading">Loading...</div>
                ) : shareError ? (
                  <div className="popup-error">{shareError}</div>
                ) : shareSuccess ? (
                  <div className="popup-success">{shareSuccess}</div>
                ) : shareMode === 'view' ? (
                  <div className="shared-tasks-content">
                    {sharedTasks.length > 0 ? (
                      <table className="shared-task-table">
                        <thead>
                          <tr>
                            <th>Title</th>
                            <th>Shared By</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                          </tr>
                        </thead>
                        <tbody>
                          {sharedTasks.map((task) => (
                            <tr key={task.id}>
                              <td>{task.title}</td>
                              <td>{task.sharedByUserName || 'Unknown'}</td>
                              <td><DateFormatter dateString={task.startDate} /></td>
                              <td><DateFormatter dateString={task.endDate} /></td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    ) : (
                      <p>No shared tasks available.</p>
                    )}
                  </div>
                ) : (
                  <div className="share-tasks-form">
                    <div className="form-group">
                      <label>Select Friend:</label>
                      <select
                          value={selectedFriendId || ''}
                          onChange={(e) => setSelectedFriendId(Number(e.target.value))}
                        >
                          <option value="">Select a user</option>
                          {friends.map(user => (
                            <option key={user.id} value={user.id}>
                              {user.name} {user.lastName}
                            </option>
                          ))}
                        </select>
                    </div>

                    <div className="form-group">
                      <label>Selected Tasks:</label>
                      {selectedTaskIds.length > 0 ? (
                        <ul className="selected-tasks-list">
                          {tasks
                            .filter(task => selectedTaskIds.includes(task.id))
                            .map(task => (
                              <li key={task.id}>{task.title}</li>
                            ))}
                        </ul>
                      ) : (
                        <p>No tasks selected</p>
                      )}
                    </div>

                    <button
                      className="share-submit-btn"
                      onClick={handleShareTask}
                      disabled={!selectedFriendId || selectedTaskIds.length === 0}
                    >
                      Share Selected Tasks
                    </button>
                  </div>
                )}

                <div className="popup-footer">
                  <button onClick={closeSharePopup} className="popup-close-btn">
                    Close
                  </button>
                </div>
              </div>
            </div>
          )}

          <div className="table-container">
            <table className="task-table">
              <thead>
                <tr className="column-labels">
                  <th></th>
                  <th>Title</th>
                  <th>Start Date</th>
                  <th>End Date</th>
                  <th>Description</th>
                  <th>Urgent</th>
                </tr>
              </thead>
              <tbody>
                {tasks.map((task) => (
                  <tr key={task.id} className="task-row">
                    <td>
                      <input 
                        type="checkbox" 
                        checked={selectedTaskIds.includes(task.id)}
                        onChange={(e) => handleCheckboxChange(task.id, e.target.checked)}
                      />
                    </td>
                    <td className="task-cell">{task.title}</td>
                    <td>
                      <div className="task-date">
                        <DateFormatter dateString={task.startDate} />
                      </div>
                    </td>
                    <td>
                      <div className="task-date">
                        <DateFormatter dateString={task.endDate} />
                      </div>
                    </td>
                    <td className="task-cell">{task.description}</td>
                    <td className="task-cell task-cell--is-urgent">
                      {task.isUrgent && <span className="urgent-indicator" title="Urgent">⚠️</span>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
      <AIChat />
    </div>
  );
};

export default TasksPage;
