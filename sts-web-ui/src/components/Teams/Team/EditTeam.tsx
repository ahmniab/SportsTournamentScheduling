import React, { useState } from 'react';
import { Team } from '../../../types/types';
import api from '../../../lib/api';
import { toast } from 'react-toastify';
import '../teams.css';

const EditTeam: React.FC<{ team: Team }> = ({ team }) => {
    const [name, setName] = useState(team.name || '');
    const [logoUrl, setLogoUrl] = useState(team.logoUrl || '');
    const [uploadedDataUrl, setUploadedDataUrl] = useState<string | null>(null);
    const [previewUrl, setPreviewUrl] = useState<string | null>(team.logoUrl || null);
    const [saving, setSaving] = useState(false);

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

    const handleSave = async (e?: React.FormEvent) => {
        e?.preventDefault();
        setSaving(true);
        try {
            const payload: any = {
                name: name.trim(),
                leagueId: team.leagueId,
                logoUrl: uploadedDataUrl || (logoUrl ? logoUrl.trim() : null),
            };
            await api.put(`/teams/${team.id}`, payload);
            toast.success('Team saved');
        } catch (err) {
            console.error('Failed to save team', err);
            toast.error('Failed to save team');
        } finally {
            setSaving(false);
        }
    };

    return (
        <form className="team-edit-form" onSubmit={handleSave}>
            <h3>Edit Team</h3>

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
                    <img src={previewUrl} alt={`Preview for ${name}`} />
                </div>
            )}

            <div className="team-actions">
                <button type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
            </div>
        </form>
    );
};

export default EditTeam;