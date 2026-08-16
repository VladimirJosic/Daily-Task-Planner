import { useNavigate } from 'react-router-dom';
import { FiLogOut } from 'react-icons/fi';
import { useTranslation } from 'react-i18next';
import './LogoutButton.css';
import { useAuth } from '../../pages/auth/AuthContext';

const LogoutButton = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('logoutButton');
  const { clearAuth } = useAuth();

  const handleLogout = () => {
    clearAuth();
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