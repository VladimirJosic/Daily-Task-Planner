import React, { useState } from 'react';
import './Login.css';
import theLogo from '../../assets/logo.png';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

const Signup = () => {
  const [name, setName] = useState("");
  const [lastName, setLastname] = useState("");
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();
  const { t } = useTranslation('signup');

  async function signup(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();

    setError("");
    setSuccess("");

    // validacija
    if (!name || !lastName || !username || !email || !password) {
      setError("All fields are required");
      return;
    }

    if (!email.includes("@") || !email.includes(".")) {
      setError("Enter a valid email address");
      return;
    }

    if (password.length < 6) {
      setError("Password must be at least 6 characters");
      return;
    }

    setIsLoading(true);

    try {
      const item = { name, lastName, username, email, password }; 

      const response = await fetch("http://localhost:3000/api/Auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(item),
      });

      if (!response.ok) {
        const contentType = response.headers.get("content-type");

        let errorMessage = "Registration failed";
        if (contentType && contentType.includes("application/json")) {
          const errorData = await response.json();
          errorMessage = errorData.message || errorMessage;
        } else {
          const text = await response.text();
          errorMessage = text || errorMessage;
        }

        throw new Error(errorMessage);
      }

      const result = await response.json();
      localStorage.setItem("user-data", JSON.stringify(result));
      setSuccess("Registration successful! Redirecting...");

      setTimeout(() => {
        navigate("/signup");
      });

    } catch (err) {
      console.error("Registration error:", err);
      setError("Registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="signup-container">
      <img src={theLogo} alt="Logo" className="logo" />
      <h1 className="signup-header">CREATE AN ACCOUNT</h1>

      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}

      <form className="signup-form" onSubmit={signup}>
        <div className="form-group">
          <input
            type="text"
            id="name"
            placeholder={t('form.name')}
            className="form-input"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>

        <div className="form-group">
          <input
            type="text"
            id="lastname"
            placeholder={t('form.lastName')}
            className="form-input"
            value={lastName}
            onChange={(e) => setLastname(e.target.value)}
          />
        </div>

        <div className="form-group">
          <input
            type="text"
            id="username"
            placeholder={t('form.username')}
            className="form-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>

        <div className="form-group">
          <input
            type="email"
            id="email"
            placeholder={t('form.email')}
            className="form-input"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="form-group">
          <input
            type="password"
            id="password"
            placeholder={t('form.password')}
            className="form-input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        <button
          type="submit"
          className="signup-button"
          disabled={isLoading}
        >
          {isLoading ? t('loading') : t('form.submit')}
        </button>
      </form>

      <p className="login-link">
        {t('form.loginLink')} <Link to="/login">{t('form.loginHere')}</Link>
      </p>
    </div>
  );
};

export default Signup;
