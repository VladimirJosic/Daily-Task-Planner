import React, { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import './Signup.css';
import theLogo from '../../assets/logo.png';
import { useTranslation } from 'react-i18next';
import { setNewPassword } from '../../apiEndpoints';

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token') ?? '';

  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');
  const [done, setDone] = useState(false);
  const { t } = useTranslation('resetPassword');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setMessage(t('messages.mismatch'));
      return;
    }

    try {
      const response = await fetch(setNewPassword, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token, newPassword: password }),
      });

      if (response.ok) {
        setDone(true);
        setMessage(t('messages.success'));
      } else if (response.status === 400) {
        // The link is unknown, already used or past its 30 minutes. The server does not
        // say which, so neither does the page.
        setMessage(t('messages.invalidToken'));
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

        {!token && <p className="message">{t('messages.missingToken')}</p>}

        {token && !done && (
          <form className="login-form" onSubmit={handleSubmit}>
            <div className="form-group">
              <input
                type="password"
                name="password"
                placeholder={t('passwordPlaceholder')}
                className="form-input"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <div className="form-group">
              <input
                type="password"
                name="confirmPassword"
                placeholder={t('confirmPlaceholder')}
                className="form-input"
                required
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />
            </div>

            <button type="submit" className="login-button">
              {t('submitButton')}
            </button>
          </form>
        )}

        {message && <p className="message">{message}</p>}

        <p className="signup-link">
          <Link to="/login">{t('loginLink')}</Link>
        </p>
      </div>
    </div>
  );
};

export default ResetPassword;
