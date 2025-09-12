import React, {useState, useEffect} from 'react';
import './UserData.css';
import theLogo from './assets/logo.png';
import { useNavigate } from 'react-router-dom';
import avatarIcon from './assets/avatar-icon.png';
import AIChat from './components/AIChat';
import { LanguageSwitcher } from './components/LanguageSwitcher/LanguageSwitcher';
import { useTranslation } from 'react-i18next';
import { getMe } from './apiEndpoints';

const UserData = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('userProfile');

  const [user, setUser] = useState<{ 
    name: string; 
    lastName: string; 
    username: string; 
    email: string 
  } | null>(null);

  const [error, setError] = useState<string>("");

  const accessToken = localStorage.getItem("accessToken");

  useEffect(() => {
    const fetchUser = async () => {
      try {
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
        setUser(data); 

      } catch (err) {
        setError("Error fetching user data");
        console.error(err);
      }
    };

    fetchUser();
  }, [navigate]);

  if (!user) return <div>{t('messages.loading')}</div>;

  return (
    <div>  
      <img src={theLogo} alt="Logo" className="logo" />
      <div className="back-button" onClick={() => navigate('/tasks')}>
        {t('buttons.back')}
      </div>

      <div className="user-container">
        <h1 className='user-header'>{t('title')}</h1>
        <img src={avatarIcon} alt="User avatar" className="avatar-icon" />

        <div className="update-button" onClick={() => navigate('/update-user')}>
          ✏️ {t('buttons.update')}
        </div>

        <div className="form-grid"> 
          <div className="form-column">
            <div className="form-group">
              <label>{t('form.name')}:</label>
              <input type="text" value={user.name} readOnly />
            </div>

            <div className="form-group">
              <label>{t('form.lastName')}:</label>
              <input type="text" value={user.lastName} readOnly />
            </div>
          </div>

          <div className="form-column">
            <div className="form-group">
              <label>{t('form.email')}:</label>
              <input type="email" value={user.email} readOnly />
            </div>

            <div className="form-group">
              <label>{t('form.username')}:</label>
              <input type="text" value={user.username} readOnly />
            </div>
          </div>
        </div>

        <div style={{ display: 'flex', justifyContent: 'center', margin: '20px auto 0' }}>
          <LanguageSwitcher />
        </div>
      </div>
      <AIChat />
    </div>
  );
};

export default UserData;