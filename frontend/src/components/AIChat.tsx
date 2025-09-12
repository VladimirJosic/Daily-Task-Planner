import React, { useState } from 'react';
import './AIChat.css';
import { queryLLM, clearLLMChatHistory } from '../apiEndpoints';

interface Message {
  text: string;
  isUser: boolean;
}

const AIChat: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [message, setMessage] = useState('');
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [messages, setMessages] = useState<Message[]>([]);

  const togglePopup = () => {
    setIsOpen(!isOpen);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleSend = async () => {
    if (message.trim()) {
      setIsLoading(true);
      
      // Add user message to chat
      setMessages(prev => [...prev, { text: message, isUser: true }]);
      setMessage(''); // Clear input immediately

      try {
        const dateOnly = selectedDate 
          ? selectedDate.toISOString().split('T')[0]
          : null;

        const response = await fetch(queryLLM, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            UserId: localStorage.getItem('userId'),
            Query: message,
            Date: dateOnly
          })
        });

        if (!response.ok) {
          throw new Error(`API request failed with status ${response.status}`);
        }

        const aiResponse = await response.text();
        
        // Add AI response to chat
        setMessages(prev => [...prev, { text: aiResponse, isUser: false }]);
        
      } catch (error) {
        console.error('Error calling LLM API:', error);
        // Add error message to chat
        setMessages(prev => [...prev, { 
          text: 'Sorry, I encountered an error. Please try again.', 
          isUser: false 
        }]);
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleClearChat = async () => {
    try {
      const response = await fetch(clearLLMChatHistory, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(localStorage.getItem('userId'))
      });

      if (!response.ok) {
        throw new Error(`Failed to clear chat history: ${response.status}`);
      }

      setMessages([]); // Clear local messages

    } catch (error) {
      console.error('Error clearing chat history:', error);
    }
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedDate(e.target.valueAsDate);
  };

  return (
    <>
      <button className="ai-button" onClick={togglePopup}>
        <span className="ai-emoji">🤖</span>
      </button>

      {isOpen && (
        <div className="ai-popup">
          <div className="ai-popup-header">
            <h3>AI Assistant</h3>
            {messages.length > 0 && (
              <button 
                className="ai-clear-button"
                onClick={handleClearChat}
                disabled={isLoading}
              >
                Clear Chat
              </button>
            )}
          </div>
          
          {/* Chat messages container */}
          {messages.length > 0 && (
            <div className="ai-messages">
              {messages.map((msg, index) => (
                <div 
                  key={index} 
                  className={`ai-chat ${msg.isUser ? 'user-message' : 'ai-message'}`}
                >
                  {msg.text}
                </div>
              ))}
              {isLoading && (
                <div className="ai-message ai-message-loading">
                  Thinking...
                </div>
              )}
            </div>
          )}

          <textarea
            placeholder="Ask me about your tasks..."
            rows={3}
            className="ai-textarea"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            onKeyDown={handleKeyDown}
            disabled={isLoading}
          />
          
          <div className="ai-input-footer">
            <span>Pick an end date (optional)</span>
            <div className="ai-date-picker-container">
              <input
                type="date"
                className="ai-date-picker"
                value={selectedDate?.toISOString().split('T')[0] || ''}
                onChange={handleDateChange}
                disabled={isLoading}
              />
            </div>
            <button 
              className="ai-send-button" 
              onClick={handleSend}
              disabled={isLoading || !message.trim()}
            >
              {isLoading ? 'Sending...' : 'Send'}
            </button>
          </div>
        </div>
      )}
    </>
  );
};

export default AIChat;