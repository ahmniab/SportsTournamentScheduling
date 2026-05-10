import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import Landing from './pages/Landing';
import LeagueDetail from './pages/LeagueDetail';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
  const apiBaseUrl = process.env.REACT_APP_API_BASE_URL?.replace(/\/$/, '');
  const loginUrl = apiBaseUrl ? `${apiBaseUrl}/login` : '/login';
  return (
    <BrowserRouter>
      <div className="App">
        <nav className="sts-nav">
          <div className="sts-brand">STS — Sports Tournaments Scheduler</div>
          <div className="sts-nav-links">
            <Link to="/">Home</Link>
            <a href={loginUrl}>Login</a>
          </div>
        </nav>

        <main className="sts-main">
          <Routes>
            <Route path="/" element={<Landing />} />
            <Route path="/leagues/:id" element={<LeagueDetail />} />
          </Routes>
          <ToastContainer position="top-right" autoClose={3000} hideProgressBar={false} newestOnTop={true} closeOnClick pauseOnHover />
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
