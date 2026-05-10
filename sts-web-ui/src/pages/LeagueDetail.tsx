import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../lib/api';
import { toast } from 'react-toastify';
import TeamsList from '../components/Teams';
import StadiumsList from '../components/Stadiums';

function isoToLocalDatetime(iso?: string) {
  if (!iso) return '';
  const d = new Date(iso);
  const tzOffset = d.getTimezoneOffset() * 60000;
  const local = new Date(d.getTime() - tzOffset);
  return local.toISOString().slice(0, 16);
}

function localDatetimeToIso(local?: string) {
  if (!local) return null;
  // local is like '2026-05-10T14:30'
  const d = new Date(local);
  return d.toISOString();
}

export default function LeagueDetail(): React.ReactElement {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [loading, setLoading] = React.useState(true);
  const [saving, setSaving] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const [name, setName] = React.useState('');
  const [startDateLocal, setStartDateLocal] = React.useState('');
  const [logoUrl, setLogoUrl] = React.useState('');
  const [uploadedDataUrl, setUploadedDataUrl] = React.useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = React.useState<string | null>(null);

  React.useEffect(() => {
    let mounted = true;
    const fetchLeague = async () => {
      if (!id) return;
      setLoading(true);
      try {
        const res = await api.get(`/Leagues/${id}`);
        if (!mounted) return;
        const data = res.data || {};
        setName(data.name || '');
        setStartDateLocal(isoToLocalDatetime(data.startDate || data.startDateUtc || ''));
        setLogoUrl(data.logoUrl || '');
        setPreviewUrl(data.logoUrl || null);
      } catch (err: any) {
        if (!mounted) return;
        setError('Failed to load league');
      } finally {
        if (!mounted) return;
        setLoading(false);
      }
    };
    void fetchLeague();
    return () => {
      mounted = false;
    };
  }, [id]);

  const handleFileChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
    const f = e.target.files && e.target.files[0];
    if (!f) return;
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      // result is a data URL like 'data:image/png;base64,...'
      setUploadedDataUrl(result);
      setPreviewUrl(result);
      // also set logoUrl so it's sent if user saves without editing the text box
      setLogoUrl(result);
    };
    reader.readAsDataURL(f);
  };

  const handleSave = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!id) return;
    setSaving(true);
    setError(null);
    try {
      const payload: any = {
        name: name.trim(),
      };
      const iso = localDatetimeToIso(startDateLocal);
      if (iso) payload.startDate = iso;
      // prefer uploaded data URL when present
      payload.logoUrl = uploadedDataUrl || (logoUrl ? logoUrl.trim() : null);
      await api.put(`/Leagues/${id}`, payload);
      // show a success toast (do not navigate away)
      toast.success('League saved successfully');
    } catch (err) {
      setError('Failed to save league');
      toast.error('Failed to save league — please try again');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div role="status">Loading league…</div>;

  return (
    <>
    <div className="league-detail">
      <h2>League</h2>
      {error && <div role="alert" className="error">{error}</div>}
      <form className="league-form" onSubmit={handleSave}>
        <label>
          Name
          <input className="text-box" value={name} onChange={(e) => setName(e.target.value)} required />
        </label>

        <label>
          Start Date (datetime)
          <input
            className="text-box"
            type="datetime-local"
            value={startDateLocal}
            onChange={(e) => setStartDateLocal(e.target.value)}
            required
          />
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
            <img src={previewUrl} alt={`Preview for ${name}`} />
          </div>
        )}

        <div className="league-actions">
          <button type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
          <button type="button" onClick={() => navigate('/')}>Cancel</button>
        </div>
      </form>
    </div>
    <TeamsList leagueId={id} />
    <StadiumsList leagueId={id} />
    </>
  );
}
