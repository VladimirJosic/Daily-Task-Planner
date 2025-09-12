import {useState} from 'react';
import './UpdateUserData.css';
import Logo from '../../assets/logo.png'
import { useNavigate } from 'react-router-dom';
import avatarIcon from '../../assets/avatar-icon.png';
import AIChat from '../../components/AIChat';

const UpdateUser = () => {
    const navigate = useNavigate();

    const [name, setName] = useState('');
    const [lastName, setLastName] = useState('');
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');

    const handleSave = () => {
    console.log('Saving changes:', { name, lastName, username, email });
    // ovde će ići poziv ka API-u u nekom sledećem koraku
  };

  return (
    <div>
      <img src={Logo} alt="Logo" className="logo" />
      <div className="back-button" onClick={() => navigate('/tasks')}>
      ← Go back to tasks
      </div>

    <div className="user-container">
      <h1 className='user-header'>UPDATE USER DATA</h1> 
      <img src={avatarIcon} alt="User avatar" className="avatar-icon" />


      <div className="form-grid">
        <div className="form-column">
          <div className="form-group">
            <input type="text" value={name} onChange={(e) => setName(e.target.value)} placeholder="Name" />
          </div>

          <div className="form-group">
            <input type="text" value={lastName} onChange={(e) => setLastName(e.target.value)} placeholder="Last Name" />
          </div>
        </div>

        <div className="form-column">
          <div className="form-group">
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
          </div>
          <div className="form-group">
            <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" />
          </div>
        </div>
      </div>

      <button className="save-button" onClick={handleSave}>
        Save Changes
      </button>
    </div>
    <AIChat/>
    </div>
  );

};
export default UpdateUser
