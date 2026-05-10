import React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../lib/api';
import {
  LeagueJobStatus,
  LeagueJobStatusResponse,
  ProtoTimestamp,
} from '../types/types';

type MonitorState =
  | 'idle'
  | 'checking'
  | 'job_not_found'
  | 'generating'
  | 'polling'
  | 'completed'
  | 'error';

function getStatusLabel(status?: LeagueJobStatus): string {
  if (status === undefined || status === null) return 'UNKNOWN';
  switch (status) {
    case LeagueJobStatus.CREATED:
      return 'CREATED';
    case LeagueJobStatus.PREPARED:
      return 'PREPARED';
    case LeagueJobStatus.GENERATING:
      return 'GENERATING';
    case LeagueJobStatus.COMPLETED:
      return 'COMPLETED';
    case LeagueJobStatus.FAILED:
      return 'FAILED';
    default:
      return 'UNKNOWN';
  }
}

function isProtoTimestamp(value: unknown): value is ProtoTimestamp {
  return (
    typeof value === 'object' &&
    value !== null &&
    'seconds' in value
  );
}

function toDisplayDate(dateValue?: string | ProtoTimestamp): string {
  if (!dateValue) return '-';

  if (isProtoTimestamp(dateValue)) {
    const seconds = Number(dateValue.seconds);
    if (Number.isNaN(seconds)) return '-';
    const millis = seconds * 1000 + Math.floor((dateValue.nanos || 0) / 1_000_000);
    return new Date(millis).toLocaleString();
  }

  const date = new Date(dateValue);
  if (Number.isNaN(date.getTime())) return '-';
  return date.toLocaleString();
}

export default function LeagueTimetableJob(): React.ReactElement {
  const navigate = useNavigate();
  const { leagueId } = useParams<{ leagueId: string }>();

  const [monitorState, setMonitorState] = React.useState<MonitorState>('idle');
  const [jobStatus, setJobStatus] = React.useState<LeagueJobStatusResponse | null>(null);
  const [lastCheckedAt, setLastCheckedAt] = React.useState<string | null>(null);
  const [lastGenerateAt, setLastGenerateAt] = React.useState<string | null>(null);
  const [error, setError] = React.useState<string | null>(null);

  const isGeneratingRef = React.useRef(false);

  const fetchJobStatus = React.useCallback(async (): Promise<LeagueJobStatusResponse | null> => {
    if (!leagueId) return null;

    try {
      const res = await api.get<LeagueJobStatusResponse>(`/Timetable/job-status/${leagueId}`);
      setJobStatus(res.data);
      setLastCheckedAt(new Date().toISOString());
      setError(null);
      return res.data;
    } catch (err: any) {
      setLastCheckedAt(new Date().toISOString());
      if (err?.response?.status === 404) {
        setMonitorState('job_not_found');
        setJobStatus(null);
        return null;
      }

      setMonitorState('error');
      setError('Failed to fetch timetable job status.');
      return null;
    }
  }, [leagueId]);

  const triggerGenerate = React.useCallback(async () => {
    if (!leagueId || isGeneratingRef.current) return;

    isGeneratingRef.current = true;
    setMonitorState('generating');

    try {
      await api.post(`/Timetable/generate/${leagueId}`);
      setLastGenerateAt(new Date().toISOString());
      setError(null);
    } catch (err: any) {
      setMonitorState('error');
      setError(
        err?.response?.status === 404
          ? 'League was not found. Unable to generate timetable.'
          : 'Failed to start timetable generation.'
      );
    } finally {
      isGeneratingRef.current = false;
    }
  }, [leagueId]);

  React.useEffect(() => {
    if (!leagueId) {
      setMonitorState('error');
      setError('Missing league id.');
      return;
    }

    let isMounted = true;

    const boot = async () => {
      setMonitorState('checking');
      const statusData = await fetchJobStatus();
      if (!isMounted) return;

      if (statusData?.status === LeagueJobStatus.COMPLETED) {
        setMonitorState('completed');
        navigate(`/league/timetable/${leagueId}`);
        return;
      }

      if (!statusData || statusData.status === LeagueJobStatus.FAILED) {
        await triggerGenerate();
      }

      if (isMounted) {
        setMonitorState('polling');
      }
    };

    void boot();

    const pollId = window.setInterval(async () => {
      if (!isMounted || !leagueId) return;

      const statusData = await fetchJobStatus();
      if (!isMounted) return;

      if (statusData?.status === LeagueJobStatus.COMPLETED) {
        setMonitorState('completed');
        navigate(`/league/timetable/${leagueId}`);
        return;
      }

      if (!statusData) {
        await triggerGenerate();
        if (isMounted) setMonitorState('polling');
        return;
      }

      if (statusData.status === LeagueJobStatus.FAILED) {
        await triggerGenerate();
      }

      if (isMounted) {
        setMonitorState('polling');
      }
    }, 5000);

    return () => {
      isMounted = false;
      window.clearInterval(pollId);
    };
  }, [fetchJobStatus, triggerGenerate, leagueId, navigate]);

  return (
    <section className="job-monitor-card" aria-live="polite">
      <h2>Timetable Job Monitor</h2>
      <p className="job-monitor-subtitle">League ID: {leagueId || '-'}</p>

      {error && <div className="error">{error}</div>}

      <table className="job-monitor-table">
        <tbody>
          <tr>
            <th>Monitor State</th>
            <td>{monitorState}</td>
          </tr>
          <tr>
            <th>Job Status</th>
            <td>{getStatusLabel(jobStatus?.status)}</td>
          </tr>
          <tr>
            <th>Created At</th>
            <td>{toDisplayDate(jobStatus?.createdAt)}</td>
          </tr>
          <tr>
            <th>Started At</th>
            <td>{toDisplayDate(jobStatus?.startedAt)}</td>
          </tr>
          <tr>
            <th>Error Message</th>
            <td>{jobStatus?.errorMessage || '-'}</td>
          </tr>
          <tr>
            <th>Last Checked</th>
            <td>{toDisplayDate(lastCheckedAt || undefined)}</td>
          </tr>
          <tr>
            <th>Last Generate Request</th>
            <td>{toDisplayDate(lastGenerateAt || undefined)}</td>
          </tr>
        </tbody>
      </table>
    </section>
  );
}
