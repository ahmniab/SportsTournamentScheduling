import React, { useEffect, useState } from 'react';
import { TimeSlot } from '../../types/types';
import api from '../../lib/api';
import './timeslots.css';
import { toast } from 'react-toastify';

const TimeSlotsList: React.FC<{ leagueId?: string }> = ({ leagueId }) => {
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingTimeSlotId, setEditingTimeSlotId] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;
    const fetchTimeSlots = async () => {
      if (!leagueId) {
        setTimeSlots([]);
        setLoading(false);
        return;
      }

      setLoading(true);
      try {
        const res = await api.get('/TimeSlots', { params: { leagueId } });
        if (!mounted) return;
        setTimeSlots(res.data || []);
      } catch (err) {
        console.error('Failed to load time slots', err);
      } finally {
        if (!mounted) return;
        setLoading(false);
      }
    };

    void fetchTimeSlots();
    return () => {
      mounted = false;
    };
  }, [leagueId]);

  if (loading) return <div role="status">Loading time slots…</div>;

  const handleDelete = async (timeSlotId: string) => {
    if (!window.confirm('Are you sure you want to delete this time slot?')) return;
    try {
      await api.delete(`/TimeSlots/${timeSlotId}`);
      toast.success('Time slot deleted');
      setTimeSlots((s) => s.filter((timeSlot) => timeSlot.id !== timeSlotId));
    } catch (err) {
      console.error('Failed to delete time slot', err);
      toast.error('Failed to delete time slot');
    }
  };

  return (
    <div className="league-detail" style={{ marginTop: '10px' }}>
      <h2>Time Slots</h2>
      <div className="team-list">
        {timeSlots.length === 0 && <div className="empty">No time slots yet</div>}
        {timeSlots.map((timeSlot) => (
          <div key={timeSlot.id} className="team">
            {editingTimeSlotId === timeSlot.id ? (
              <>
                <EditTimeSlotForm
                  timeSlot={timeSlot}
                  onSaved={(updated) => {
                    setTimeSlots((current) => current.map((item) => (item.id === updated.id ? updated : item)));
                    setEditingTimeSlotId(null);
                  }}
                  onCancel={() => setEditingTimeSlotId(null)}
                />
              </>
            ) : (
              <>
                <ShowTimeSlot timeSlot={timeSlot} />
                <div className="team-actions">
                  <button className="btn" onClick={() => setEditingTimeSlotId(timeSlot.id)}>Edit</button>
                  <button className="btn" style={{ backgroundColor: 'red' }} onClick={() => handleDelete(timeSlot.id)}>Delete</button>
                </div>
              </>
            )}
          </div>
        ))}

        {(timeSlots.length < 3) &&(
          <div className="team team-create">
            <CreateTimeSlotForm leagueId={leagueId} onCreated={(timeSlot) => setTimeSlots((prev) => [...prev, timeSlot])} />
          </div>
        )}
      </div>
    </div>
  );
};

type TimeSlotFormProps = {
  leagueId?: string;
  timeSlot?: TimeSlot;
  onCreated?: (timeSlot: TimeSlot) => void;
  onSaved?: (timeSlot: TimeSlot) => void;
  onCancel?: () => void;
};

const TimeSlotFields: React.FC<{
  startTime: string;
  endTime: string;
  setStartTime: (value: string) => void;
  setEndTime: (value: string) => void;
}> = ({ startTime, endTime, setStartTime, setEndTime }) => {
  return (
    <>
      <label>
        Start Time
        <input 
            className="text-box" 
            type="text" 
            value={startTime} 
            onChange={(e) => setStartTime(e.target.value)} 
            required 
        />
      </label>

      <label>
        End Time
        <input 
            className="text-box" 
            type="text" 
            value={endTime} onChange={(e) => setEndTime(e.target.value)} 
            required 
        />
      </label>
    </>
  );
};

const CreateTimeSlotForm: React.FC<TimeSlotFormProps> = ({ leagueId, onCreated }) => {
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [creating, setCreating] = useState(false);

  const handleCreate = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!leagueId) return toast.error('League is required');
    if (!startTime || !endTime) return toast.error('Start and end time are required');

    setCreating(true);
    try {
      const payload = {
        leagueId,
        startTime: startTime,
        endTime: endTime,
      };

      const res = await api.post('/TimeSlots', payload);
      const created: TimeSlot = res.data;
      toast.success('Time slot created');
      setStartTime('');
      setEndTime('');
      onCreated && onCreated(created);
    } catch (err) {
      console.error('Failed to create time slot', err);
      toast.error('Failed to create time slot');
    } finally {
      setCreating(false);
    }
  };

  return (
    <form className="team-edit-form" onSubmit={handleCreate} aria-label="Create time slot">
      <h3>Create Time Slot</h3>
      <TimeSlotFields
        startTime={startTime}
        endTime={endTime}
        setStartTime={setStartTime}
        setEndTime={setEndTime}
      />

      <div className="team-actions">
        <button className="btn" type="submit" disabled={creating}>{creating ? 'Creating…' : 'Create'}</button>
      </div>
    </form>
  );
};

const EditTimeSlotForm: React.FC<TimeSlotFormProps> = ({ timeSlot, onSaved, onCancel }) => {
  const [startTime, setStartTime] = useState(timeSlot?.startTime);
  const [endTime, setEndTime] = useState(timeSlot?.endTime);
  const [saving, setSaving] = useState(false);

  const handleSave = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!timeSlot) return;
    if (!startTime || !endTime) return toast.error('Start and end time are required');

    setSaving(true);
    try {
      const payload = {
        startTime: startTime,
        endTime: endTime,
      };

      const res = await api.put(`/TimeSlots/${timeSlot.id}`, payload);
      const updated: TimeSlot = res.data;
      toast.success('Time slot saved');
      onSaved && onSaved(updated);
    } catch (err) {
      console.error('Failed to save time slot', err);
      toast.error('Failed to save time slot');
    } finally {
      setSaving(false);
    }
  };

  if (!timeSlot) return null;

  return (
    <form className="team-edit-form" onSubmit={handleSave} aria-label="Edit time slot">
      <h3>Edit Time Slot</h3>
      <TimeSlotFields
        startTime={startTime ?? ''}
        endTime={endTime ?? ''}
        setStartTime={setStartTime}
        setEndTime={setEndTime}
      />

      <div className="team-actions">
        <button className="btn" type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
        <button className="btn btn-secondary" type="button" onClick={onCancel}>Close</button>
      </div>
    </form>
  );
};

const ShowTimeSlot: React.FC<{ timeSlot: TimeSlot }> = ({ timeSlot }) => {
  return (
    <div className="team-card">
      <div className="team-card-body">
        <h3 className="team-name">Time Slot</h3>
        <div className="team-meta">Start: {timeSlot.startTime}</div>
        <div className="team-meta">End: {timeSlot.endTime}</div>
      </div>
    </div>
  );
};

export default TimeSlotsList;