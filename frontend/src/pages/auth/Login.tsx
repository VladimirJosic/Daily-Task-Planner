import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Login.css';
import { login } from '../../apiEndpoints';
import theLogo from '../../assets/logo.png';
import { useTranslation } from 'react-i18next';

const Login = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('login');
  
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  async function handleLogin(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!username || !password) {
      setError(t('errors.required'));
      return;
    }

    setIsLoading(true);

    try {
      const loginResponse = await fetch(login, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ username, password })
      });

      if (!loginResponse.ok) {
        const contentType = loginResponse.headers.get("content-type");

        let errorMessage = t('errors.invalidCredentials');
        if (contentType && contentType.includes("application/json")) {
          const errorData = await loginResponse.json();
          errorMessage = errorData.message || errorMessage;
        } else {
          const text = await loginResponse.text();
          errorMessage = text || errorMessage;
        }

        throw new Error(errorMessage);
      }

      const result = await loginResponse.json();

      localStorage.setItem("userId", result.userId.toString());
      localStorage.setItem("accessToken", result.accessToken);
      localStorage.setItem("refreshToken", result.refreshToken);
      localStorage.setItem("loginTime", new Date().getTime().toString());
      
      setSuccess(t('success.login'));

      setTimeout(() => {
        navigate("/tasks", {
          state: {
            id: result.userId,
            username: result.username
          }
        });
      });

    } catch (err) {
      console.error("Login error:", err);
      setError(t('errors.loginFailed'));
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="page login-background">
      <div className="login-container">
        <img src={theLogo} alt="Logo" className="logo" />
        <h1 className="login-header">{t('title')}</h1>

        {error && <div className="alert alert-error">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <form className="login-form" onSubmit={handleLogin}>
          <div className="form-group">
            <input
              type="text"
              placeholder={t('username')}
              className="form-input"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <input
              type="password"
              placeholder={t('password')}
              className="form-input"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <p className="forgot-password">
            <Link to="/forgot-password">{t('forgotPassword')}</Link>
          </p>

          <button type="submit" className="login-button" disabled={isLoading}>
            {isLoading ? t('loading') : t('submit')}
          </button>
        </form>

        <p className="signup-link">
          {t('noAccount')} <Link to="/signup">{t('signUp')}</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;