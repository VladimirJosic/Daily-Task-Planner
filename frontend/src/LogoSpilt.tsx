import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import theLogo from './assets/logo.png';
import './App.css';

const LogoSplit: React.FC = () => {
  const navigate = useNavigate();
  const [isSplit, setIsSplit] = useState(false);
  const [hideWrapper, setHideWrapper] = useState(false);

  useEffect(() => {
    const checkAuthStatus = () => {
      const accessToken = localStorage.getItem('accessToken');
      const userId = localStorage.getItem('userId');
      return accessToken && userId;
    };

    const splitTimeout = setTimeout(() => {
      setIsSplit(true);
    }, 100);

    const hideTimeout = setTimeout(() => {
      setHideWrapper(true);
    }, 1000 + 1500); // 1.5s trajanje animacije

    const navTimeout = setTimeout(() => {
      if (checkAuthStatus()) {
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
  }, [navigate]);

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