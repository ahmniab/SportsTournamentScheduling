import React, { useEffect, useState } from 'react';
import { Stadium } from '../../types/types';
import api from '../../lib/api';
import ShowStadium from './Stadium/ShowStadium';
import EditStadium from './Stadium/EditStadium';
import './stadiums.css';
import { toast } from 'react-toastify';

const StadiumsList: React.FC<{ leagueId?: string }> = ({ leagueId }) => {
  const [stadiums, setStadiums] = useState<Stadium[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingStadiumId, setEditingStadiumId] = useState<number | string | null>(null);

  useEffect(() => {
    let mounted = true;
    const fetch = async () => {
      setLoading(true);
      try {
        const url = leagueId ? `/Stadiums/getByLeagueId/${leagueId}` : '/Stadiums';
        const res = await api.get(url);
        if (!mounted) return;
        setStadiums(res.data || []);
      } catch (err) {
        console.error('Failed to load stadiums', err);
      } finally {
        if (!mounted) return;
        setLoading(false);
      }
    };
    void fetch();
    return () => { mounted = false; };
  }, [leagueId]);

  if (loading) return <div role="status">Loading stadiums…</div>;

  const handleDelete = async (id: number | string) => {
    if (!window.confirm('Are you sure you want to delete this stadium?')) return;
    try {
      await api.delete(`/Stadiums/${id}`);
      toast.success('Stadium deleted');
      setStadiums((s) => s.filter((st) => st.id !== id));
    } catch (err) {
      console.error('Failed to delete stadium', err);
      toast.error('Failed to delete stadium');
    }
  };

  return (
    <div className="league-detail" style={{ marginTop: '10px' }}>
      <h2>Stadiums</h2>
      <div className="team-list">
        {stadiums.length === 0 && <div className="empty">No stadiums yet</div>}
        {stadiums.map((stadium) => (
          <div key={stadium.id} className="team">
            {editingStadiumId === stadium.id ? (
              <>
                <EditStadium stadium={stadium} />
                <div className="team-actions">
                  <button className="btn btn-secondary" onClick={() => setEditingStadiumId(null)}>Close</button>
                </div>
              </>
            ) : (
              <>
                <ShowStadium stadium={stadium} />
                <div className="team-actions">
                  <button className="btn" onClick={() => setEditingStadiumId(stadium.id)}>Edit</button>
                  <button className="btn" style={{ backgroundColor: 'red'}} onClick={() => handleDelete(stadium.id)}>Delete</button>
                </div>
              </>
            )}
          </div>
        ))}

        <div className="team team-create">
          <CreateStadiumForm leagueId={leagueId} onCreated={(s) => setStadiums((prev) => [...prev, s])} />
        </div>
      </div>
    </div>
  );
};

type CreateProps = { leagueId?: string; onCreated?: (s: Stadium) => void };

const CreateStadiumForm: React.FC<CreateProps> = ({ leagueId, onCreated }) => {
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
        logo: uploadedDataUrl || (logoUrl ? logoUrl.trim() : null),
      };
      if (leagueId) payload.leagueId = leagueId;
      const res = await api.post('/Stadiums', payload);
      const created: Stadium = res.data;
      toast.success('Stadium created');
      setName('');
      setLogoUrl('');
      setPreviewUrl(null);
      setUploadedDataUrl(null);
      onCreated && onCreated(created);
    } catch (err) {
      console.error('Failed to create stadium', err);
      toast.error('Failed to create stadium');
    } finally {
      setCreating(false);
    }
  };

  return (
    <form className="team-edit-form" onSubmit={handleCreate} aria-label="Create stadium">
      <h3>Create Stadium</h3>
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
          <img src={previewUrl} alt={`Preview for ${name || 'new stadium'}`} />
        </div>
      )}

      <div className="team-actions">
        <button className="btn" type="submit" disabled={creating}>{creating ? 'Creating…' : 'Create'}</button>
      </div>
    </form>
  );
};

export default StadiumsList;
