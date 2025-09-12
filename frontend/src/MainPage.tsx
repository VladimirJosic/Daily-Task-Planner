import './MainPage.css'; // bolje da se CSS za ovu stranicu zove tako
import theLogo from './assets/logo.png';

const MainPage = () => {
  return (
    <div className="main-page">


      <img src={theLogo} alt="Logo" className="logo" />
   
    </div>
  );
};

export default MainPage;
