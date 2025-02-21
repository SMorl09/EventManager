import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import EventsTable from './pages/EventsTable';
import EventDetails from './pages/EventDetails';
import CreateEvent from './pages/CreateEvent';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!sessionStorage.getItem('token'));

  useEffect(() => {
      const handleStorageChange = () => {
          setIsAuthenticated(!!sessionStorage.getItem('token'));
      };

      window.addEventListener('storage', handleStorageChange);

      return () => {
          window.removeEventListener('storage', handleStorageChange);
      };
  }, []);
    return (
      <Router>
        <Routes>
          <Route path="/" element={<Login setIsAuthenticated={setIsAuthenticated} />} />
          <Route path="/login" element={<Login setIsAuthenticated={setIsAuthenticated} />} />
          <Route path="/register" element={<Register />} />
          <Route path="/events" element={isAuthenticated ? <EventsTable /> : <Navigate to="/login" />} />
          <Route path="/events/:id" element={isAuthenticated ? <EventDetails /> : <Navigate to="/login" />} />
          <Route path="/create-event" element={isAuthenticated ? <CreateEvent />: <Navigate to="/login" />} />
       </Routes>
      </Router>
    );
}

export default App;
