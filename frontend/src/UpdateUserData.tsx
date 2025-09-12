import React, { useEffect, useState } from 'react';
import theLogo from './assets/logo.png';
import { useNavigate } from 'react-router-dom';
import avatarIcon from './assets/avatar-icon.png';
import AIChat from './components/AIChat';
import { getMe } from './apiEndpoints';

const UpdateUser = () => {
  const navigate = useNavigate();

  const [userId, setUserId] = useState<number | null>(null);
  const [name, setName] = useState('');
  const [lastName, setLastName] = useState('');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState<string>('');

   const accessToken = localStorage.getItem("accessToken");

  useEffect(() => {
    const fetchCurrentUserIdAndData = async () => {
      try {
        // Dohvati ID korisnika
        const res = await fetch(`${getMe}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        });

        if (!res.ok) {
          setError("Failed to fetch user data");
          return;
        }

        const data = await res.json();
        console.log("User ID:", data.id);
        
      // Dohvati detaljne podatke korisnika po ID-ju
        setName(data.name || '');
        setLastName(data.lastName || '');
        setUsername(data.username || '');
        setEmail(data.email || '');

      } catch (err) {
        setError("Error fetching user data");
        console.error(err);
      }
    };

    fetchCurrentUserIdAndData();
  }, []);

  const handleSave = async () => {
  try {
    const res = await fetch(`http://localhost:3000/api/users/update`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${accessToken}`
      },
      body: JSON.stringify({
        name,
        lastName,
        username,
        email
      })
    });

    if (!res.ok) {
      alert("Failed to update user data");
      return;
    }

    alert("User data updated!");
    navigate("/tasks");

  } catch (err) {
    console.error("Error updating user data:", err);
    alert("An error occurred.");
  }
};

  return (
    <div>
      <img src={theLogo} alt="Logo" className="logo" />
      <div className="back-button" onClick={() => navigate('/tasks')}>
        ← Go back to tasks
      </div>

      <div className="user-container">
        <h1 className="user-header">UPDATE USER DATA</h1>
        <img src={avatarIcon} alt="User avatar" className="avatar-icon" />

        {error && <div style={{ color: 'red', marginBottom: 10 }}>{error}</div>}

        <div className="form-grid">
          <div className="form-column">
            <div className="form-group">
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Name"
              />
            </div>
            <div className="form-group">
              <input
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                placeholder="Last Name"
              />
            </div>
          </div>

          <div className="form-column">
            <div className="form-group">
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email"
              />
            </div>
            <div className="form-group">
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Username"
              />
            </div>
          </div>
        </div>

        <button className="save-button" onClick={handleSave}>
          Save Changes
        </button>
      </div>

      <AIChat />
    </div>
  );
};

export default UpdateUser;
