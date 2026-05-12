import React from 'react';
import api from '../lib/api';
import { FiPlus, FiTrash2 } from 'react-icons/fi';

export default function LeaguesPanel(): React.ReactElement {
  const [leagues, setLeagues] = React.useState<any[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<any | null>(null);
  const [creating, setCreating] = React.useState(false);
  const [newName, setNewName] = React.useState('');

  React.useEffect(() => {
    let mounted = true;
    const fetchLeagues = async () => {
      setLoading(true);
      try {
        const res = await api.get('/Leagues');
        if (!mounted) return;
        setLeagues(res.data || []);
      } catch (err) {
        if (!mounted) return;
        setError(err);
      } finally {
        if (!mounted) return;
        setLoading(false);
      }
    };
    void fetchLeagues();
    return () => {
      mounted = false;
    };
  }, []);

  if (loading) return <div role="status">Loading your leagues…</div>;
  console.log('leagues', leagues);
  if (error) return <div role="alert">Failed to load leagues.</div>;
  if (leagues.length === 0)
    return (
      <div className="leagues-empty">
        <form
        className="league-create"
        onSubmit={async (e) => {
          e.preventDefault();
          if (!newName.trim()) return;
          setCreating(true);
          try {
            const payload = { name: newName.trim(), startDate: new Date().toISOString() };
            const res = await api.post('/Leagues', payload);
            const created = res.data;
            setLeagues((s) => [created, ...s]);
            setNewName('');
          } catch (err) {
            // keep simple: set error so user sees something
            setError(err);
          } finally {
            setCreating(false);
          }
        }}
      >
        <input
          className="text-box"
          aria-label="New league name"
          placeholder="New league name"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
        />
        <button type="submit" disabled={creating || !newName.trim()} title="Create league">
          {React.createElement(FiPlus as unknown as React.ComponentType<any>)} Create
        </button>
      </form>
        <h2>No leagues yet</h2>
        <p>Create your first league to get started.</p>
      </div>
    );

  const getInitials = (name: string | undefined) => {
    if (!name) return 'L';
    return name
      .split(' ')
      .map((p) => p[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  };

  return (
    <section className="leagues">
      <h2>Your Leagues</h2>
      <form
        className="league-create"
        onSubmit={async (e) => {
          e.preventDefault();
          if (!newName.trim()) return;
          setCreating(true);
          try {
            const payload = { name: newName.trim(), startDate: new Date().toISOString() };
            const res = await api.post('/Leagues', payload);
            const created = res.data;
            setLeagues((s) => [created, ...s]);
            setNewName('');
          } catch (err) {
            // keep simple: set error so user sees something
            setError(err);
          } finally {
            setCreating(false);
          }
        }}
      >
        <input
          className="text-box"
          aria-label="New league name"
          placeholder="New league name"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
        />
        <button type="submit" disabled={creating || !newName.trim()} title="Create league">
          {React.createElement(FiPlus as unknown as React.ComponentType<any>)} Create
        </button>
      </form>
      <ul className="leagues-list">
        {leagues.map((l: any) => (
          <li key={l.id} className="league-item">
            <a className="league-card" href={`/leagues/${l.id}`} aria-label={`Open league ${l.name}`}>
              <div className="league-logo">
                {l.logoUrl ? (
                  // image has pointer-events none so clicks fall through to parent anchor
                  // alt kept empty because the league name is the accessible label
                  <img src={l.logoUrl} alt="" />
                ) : (
                  <div className="league-logo-placeholder">{getInitials(l.name)}</div>
                )}
              </div>
              <div className="league-body">
                <h3 className="league-name">{l.name || 'Untitled League'}</h3>
                {l.description && <p className="league-desc">{l.description}</p>}
              </div>
            </a>
            <button
              className="league-delete"
              aria-label={`Delete league ${l.name}`}
              onClick={async (e) => {
                e.preventDefault();
                // confirm before delete
                if (!window.confirm(`Delete league “${l.name}”? This cannot be undone.`)) return;
                try {
                  await api.delete(`/Leagues/${l.id}`);
                  setLeagues((s) => s.filter((x) => x.id !== l.id));
                } catch (err) {
                  setError(err);
                }
              }}
              title="Delete league"
            >
              {React.createElement(FiTrash2 as unknown as React.ComponentType<any>)}
            </button>
          </li>
        ))}
      </ul>
    </section>
  );
}
