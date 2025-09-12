import React from 'react';
import { useTranslation } from 'react-i18next';
import { FiChevronDown } from 'react-icons/fi';
import './LanguageSwitcher.css';
import { GB, RS } from 'country-flag-icons/react/3x2';

type LanguageOption = {
  code: string;
  name: string;
  icon: React.ReactNode;
};

const LANGUAGE_OPTIONS: LanguageOption[] = [
  { 
    code: 'en', 
    name: 'English', 
    icon: <GB title="English" className="flag-icon" /> 
  },
  { 
    code: 'sr', 
    name: 'Srpski', 
    icon: <RS title="Srpski" className="flag-icon" /> 
  },
];

export const LanguageSwitcher: React.FC = () => {
  const { i18n } = useTranslation();

  const handleLanguageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    i18n.changeLanguage(e.target.value);
  };

  const currentLanguage = LANGUAGE_OPTIONS.find(lang => lang.code === i18n.language) || LANGUAGE_OPTIONS[0];

  return (
    <div className="language-switcher-container">
      <div className="current-language">
        {currentLanguage.icon}
        <span className="name">{currentLanguage.name}</span>
        <FiChevronDown className="chevron" />
      </div>
      <select
        value={i18n.language}
        onChange={handleLanguageChange}
        className="language-select"
        aria-label="Select language"
      >
        {LANGUAGE_OPTIONS.map((lang) => (
          <option key={lang.code} value={lang.code}>
            {lang.name}
          </option>
        ))}
      </select>
    </div>
  );
};