import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './Signup.css';
import theLogo from '../../assets/logo.png';
import { useTranslation } from 'react-i18next';

const Password = () => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const { t } = useTranslation('forgotPassword');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const response = await fetch(
        `http://localhost:3000/api/Auth/reset-password?email=${encodeURIComponent(email)}`,
        {
          method: 'POST',
        }
      );

      if (response.ok) {
        setMessage(t('messages.success'));
      } else {
        setMessage(t('messages.error'));
      }
    } catch (error) {
      console.error('Error:', error);
      setMessage(t('messages.genericError'));
    }
  };

  return (
    <div className="page login-background">
      <div className="login-container">
        <img src={theLogo} alt="Logo" className="logo" />

        <h1 className="login-header">{t('title')}</h1>

        <form className="login-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <input
              type="email"
              placeholder={t('emailPlaceholder')}
              className="form-input"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <button type="submit" className="login-button">
            {t('submitButton')}
          </button>
        </form>

        {message && <p className="message">{message}</p>}

        <p className="signup-link">
          {t('rememberPassword')} <Link to="/login">{t('loginLink')}</Link>
        </p>
      </div>
    </div>
  );
};

export default Password;