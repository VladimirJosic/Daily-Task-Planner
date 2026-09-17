// App.tsx
import { Routes, Route } from 'react-router-dom'
import LogoSplit from './LogoSpilt.tsx'
import MainPage from './MainPage.tsx'
import Week from './Week.tsx'
import UpdateUser from './UpdateUserData.tsx';
import UserData from './UserData.tsx';
import Month from './Month.tsx'
import Password from './pages/auth/Password.tsx';
import ResetPassword from './pages/auth/ResetPassword.tsx';
import AddTask from './pages/tasks/AddTask.tsx';
import TasksPage from './pages/tasks/TasksPage.tsx';
import Signup from './pages/auth/Signup.tsx';
import Login from './pages/auth/Login.tsx';

const App = () => {
  return (
    <Routes>
          <Route path="/" element={<LogoSplit />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/login" element={<Login />} />
          <Route path="/forgot-password" element={<Password />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/mainpage" element={<MainPage />} />
          <Route path="/week" element={<Week />} />
          <Route path="/update-user" element={<UpdateUser />} />
          <Route path="/user" element={<UserData />} />
          <Route path="/add-task" element={<AddTask />} />
          <Route path="/tasks" element={<TasksPage />} />
          <Route path="/month" element={<Month />} />
    </Routes>
  )
}

export default App
