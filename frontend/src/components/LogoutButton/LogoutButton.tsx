import { useNavigate } from 'react-router-dom';
import { FiLogOut } from 'react-icons/fi';
import { useTranslation } from 'react-i18next';
import './LogoutButton.css';

const LogoutButton = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('logoutButton');

  const handleLogout = () => {
    localStorage.removeItem("userId");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("loginTime");
    navigate("/login");
  };

  return (
    <button 
      onClick={handleLogout} 
      className="logout-button"
      aria-label={t('ariaLabel')}
    >
      <span>{t('button')}</span>
      <FiLogOut className="logout-icon" />
    </button>
  );
};

export default LogoutButton;