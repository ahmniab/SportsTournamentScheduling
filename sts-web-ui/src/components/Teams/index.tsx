import React, { useEffect, useState } from 'react';
import { Team } from '../../types/types';
import api from '../../lib/api';
import ShowTeam from './Team/ShowTeam';
import EditTeam from './Team/EditTeam';
import './teams.css';
import { toast } from 'react-toastify';

const TeamsList: React.FC<{ leagueId?: string }> = ({ leagueId }) => {
  const [teams, setTeams] = useState<Team[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingTeamId, setEditingTeamId] = useState<number | string | null>(null);

  useEffect(() => {
    let mounted = true;
    const fetchTeams = async () => {
      setLoading(true);
      try {
        const url = leagueId ? `/Teams/getByLeagueId/${leagueId}` : '/teams';
        const res = await api.get(url);
        if (!mounted) return;
        setTeams(res.data || []);
      } catch (err) {
        console.error('Failed to load teams', err);
      } finally {
        if (!mounted) return;
        setLoading(false);
      }
    };
    void fetchTeams();
    return () => { mounted = false; };
  }, [leagueId]);

  if (loading) return <div role="status">Loading teams…</div>;

  const handleDelete = async (teamId: number | string) => {
    if (!window.confirm('Are you sure you want to delete this team?')) return;
    try {
      await api.delete(`/Teams/${teamId}`);
      toast.success('Team deleted');
      setTeams((s) => s.filter((t) => t.id !== teamId));
    } catch (err) {
      console.error('Failed to delete team', err);
      toast.error('Failed to delete team');
    }
  };

  return (
    <div className="league-detail" style={{ marginTop: '10px' }}>
    <h2>Teams</h2>
    <div className="team-list">
      {teams.length === 0 && <div className="empty">No teams yet</div>}
      {teams.map((team) => (
        <div key={team.id} className="team">
          {editingTeamId === team.id ? (
            <>
              <EditTeam team={team} />
              <div className="team-actions">
                <button className="btn btn-secondary" onClick={() => setEditingTeamId(null)}>Close</button>
              </div>
            </>
          ) : (
            <>
              <ShowTeam team={team} />
              <div className="team-actions">
                <button className="btn" onClick={() => setEditingTeamId(team.id)}>Edit</button>
                <button className="btn" style={{ backgroundColor: 'red'}} onClick={() => handleDelete(team.id)}>Delete</button>
              </div>
            </>
          )}
        </div>
      ))}
      {/* Create team form at end of list */}
      <div className="team team-create">
        <CreateTeamForm leagueId={leagueId} onCreated={(t) => setTeams((s) => [...s, t])} />
      </div>
    </div>
    </div>
  );
};


type CreateTeamFormProps = {
  leagueId?: string;
  onCreated?: (t: Team) => void;
};

const CreateTeamForm: React.FC<CreateTeamFormProps> = ({ leagueId, onCreated }) => {
  const [name, setName] = useState('');
  const [logoUrl, setLogoUrl] = useState('');
  const [uploadedDataUrl, setUploadedDataUrl] = useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);

  const handleFileChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
    const f = e.target.files && e.target.files[0];
    if (!f) return;
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      setUploadedDataUrl(result);
      setPreviewUrl(result);
      setLogoUrl(result);
    };
    reader.readAsDataURL(f);
  };

  const handleCreate = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!name.trim()) return toast.error('Name required');
    setCreating(true);
    try {
      const payload: any = {
        name: name.trim(),
        logoUrl: uploadedDataUrl || (logoUrl ? logoUrl.trim() : null),
      };
      if (leagueId) payload.leagueId = leagueId;
      const res = await api.post('/teams', payload);
      const created: Team = res.data;
      toast.success('Team created');
      setName('');
      setLogoUrl('');
      setPreviewUrl(null);
      setUploadedDataUrl(null);
      onCreated && onCreated(created);
    } catch (err) {
      console.error('Failed to create team', err);
      toast.error('Failed to create team');
    } finally {
      setCreating(false);
    }
  };

  return (
    <form className="team-edit-form" onSubmit={handleCreate} aria-label="Create team">
      <h3>Create Team</h3>
      <label>
        Name
        <input className="text-box" value={name} onChange={(e) => setName(e.target.value)} required />
      </label>

      <label>
        Logo URL
        <input
          className="text-box"
          placeholder="https://... or leave blank to upload"
          value={logoUrl}
          onChange={(e) => {
            setLogoUrl(e.target.value);
            setUploadedDataUrl(null);
            setPreviewUrl(e.target.value || null);
          }}
        />
      </label>

      <label>
        Or upload logo
        <input className="text-box" type="file" accept="image/*" onChange={handleFileChange} />
      </label>

      {previewUrl && (
        <div className="logo-preview">
          <img src={previewUrl} alt={`Preview for ${name || 'new team'}`} />
        </div>
      )}

      <div className="team-actions">
        <button className="btn" type="submit" disabled={creating}>{creating ? 'Creating…' : 'Create'}</button>
      </div>
    </form>
  );
};

export default TeamsList;
