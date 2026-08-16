import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import theLogo from './assets/logo.png';
import './App.css';
import { useAuth } from './pages/auth/AuthContext';

const LogoSplit: React.FC = () => {
  const navigate = useNavigate();
  const [isSplit, setIsSplit] = useState(false);
  const [hideWrapper, setHideWrapper] = useState(false);
  const { accessToken, isAuthLoading } = useAuth();

  useEffect(() => {
    if (isAuthLoading) return;

    const splitTimeout = setTimeout(() => {
      setIsSplit(true);
    }, 100);

    const hideTimeout = setTimeout(() => {
      setHideWrapper(true);
    }, 2500);

    const navTimeout = setTimeout(() => {
      if (accessToken) {
        navigate('/tasks');
      } else {
        navigate('/login');
      }
    }, 2000);

    return () => {
      clearTimeout(splitTimeout);
      clearTimeout(hideTimeout);
      clearTimeout(navTimeout);
    };
  }, [navigate, accessToken, isAuthLoading]);

  if (hideWrapper) {
    return null;
  }

  return (
    <div className="logo-split-wrapper">
      <div className={`half left ${isSplit ? 'split-left' : ''}`}>
        <img src={theLogo} alt="Logo left half" />
      </div>
      <div className={`half right ${isSplit ? 'split-right' : ''}`}>
        <img src={theLogo} alt="Logo right half" />
      </div>
    </div>
  );
};

export default LogoSplit;