import React from 'react';

interface DateFormatterProps {
  dateString: string | Date;
  showTime?: boolean;
}

const DateFormatter: React.FC<DateFormatterProps> = ({ dateString, showTime = true }) => {
  const formatDate = (date: Date) => {
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    
    if (!showTime) {
      return `${day}/${month}/${year}`;
    }

    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    
    return `${day}/${month}/${year} ${hours}:${minutes}`;
  };

  try {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) {
      throw new Error('Invalid date');
    }

    return (
      <span className="formatted-date">
        {formatDate(date)}
      </span>
    );
  } catch (error) {
    console.error('Date formatting error:', error);
    return <span className="date-error">Invalid date</span>;
  }
};

export default DateFormatter;