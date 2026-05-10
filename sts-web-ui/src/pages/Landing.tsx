import React from 'react';
import '../App.css';
import useAuth from '../hooks/useAuth';
import LeaguesPanel from '../components/LeaguesPanel';

export default function Landing() {
  const apiBaseUrl = process.env.REACT_APP_API_BASE_URL?.replace(/\/$/, '');
  const loginUrl = apiBaseUrl ? `${apiBaseUrl}/login` : '/login';
  const { user, loading, isAuthenticated } = useAuth();

  if (loading) {
    return (
      <div className="landing">
        <section className="hero">
          <h1 className="hero-title">Sports Tournaments Scheduler</h1>
          <p className="hero-sub">Checking authentication…</p>
        </section>
      </div>
    );
  }

  if (isAuthenticated) {
    return (
      <div className="landing">
        <header className="hero">
          <h1 className="hero-title">Welcome back{user?.name ? `, ${user.name}` : ''}</h1>
          <p className="hero-sub">Here are your leagues.</p>
        </header>
        <main>
          <LeaguesPanel />
        </main>
        <footer className="sts-footer">© {new Date().getFullYear()} STS — Built for sports organizers</footer>
      </div>
    );
  }

  return (
    <div className="landing">
      <section className="hero">
        <h1 className="hero-title">Sports Tournaments Scheduler</h1>
        <p className="hero-sub">AI-assisted scheduling for football, basketball, and more.</p>
        <div className="hero-cta">
          <a className="btn primary" href={loginUrl}>Get Started — Login</a>
        </div>
      </section>

      <section className="features">
        <div className="feature">
          <h3>Fast Schedules</h3>
          <p>Auto-generate conflict-free timetables in minutes.</p>
        </div>
        <div className="feature">
          <h3>Smart Rules</h3>
          <p>Custom constraints for venues, teams, and referees.</p>
        </div>
        <div className="feature">
          <h3>Cross-Sport</h3>
          <p>Built for football, basketball, volleyball and more.</p>
        </div>
      </section>

      <footer className="sts-footer">© {new Date().getFullYear()} STS — Built for sports organizers</footer>
    </div>
  );
}
