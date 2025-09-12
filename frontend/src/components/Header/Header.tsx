import { Link } from "react-router-dom";
import theLogo2 from "../../assets/logo.png";
import LogoutButton from "../LogoutButton/LogoutButton";
import "./Header.css";
import { useTranslation } from "react-i18next";
import { useNavigate } from 'react-router-dom';

const Header = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('header');

  return (
    <div className="header-container">
      <div className="header">
        <img 
          onClick={() => navigate('/tasks')} 
          src={theLogo2} 
          alt="logo" 
          className="logo2" 
          style={{ cursor: 'pointer' }}
        />
        
        <h2 className="header-title">{t('title')}</h2>

        <div className="header-right">
          <div className="header-buttons">
            <Link to="/week" style={{ textDecoration: "none" }}>
              <button className="header-mode-btn">
                {t('buttons.week')}
              </button>
            </Link>
            <Link to="/month" style={{ textDecoration: "none" }}>
              <button className="header-mode-btn">
                {t('buttons.month')}
              </button>
            </Link>
          </div>

          <Link 
            to="/user" 
            className="user-profile"
            aria-label={t('ariaLabels.userProfile')}
          >
            <svg
              height="32"
              viewBox="0 0 24 24"
              width="32"
              fill="#a472d8"
            >
              <circle cx="12" cy="8" r="4" />
              <path d="M12 14c-4 0-8 2-8 4v2h16v-2c0-2-4-4-8-4z" />
            </svg>
          </Link>
          <LogoutButton />
        </div>
      </div>
    </div>
  );
};

export default Header;