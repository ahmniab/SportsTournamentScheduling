import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../lib/api';
import { LeagueJobStatus, LeagueJobStatusResponse } from '../types/types';

interface LeagueGenerationProps {
  leagueId: string;
}

export const LeagueGeneration: React.FC<LeagueGenerationProps> = ({ leagueId }) => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [jobStatus, setJobStatus] = useState<LeagueJobStatusResponse | null>(null);
  const [genBtnTxt, setGenBtnTxt] = useState('Generate League');

  // Fetch job status on component mount to determine button color
  useEffect(() => {
    const fetchJobStatus = async () => {
      try {
        const response = await api.get<LeagueJobStatusResponse>(
          `/timetable/job-status/${leagueId}`
        );
        setJobStatus(response.data);
        if (jobStatus?.status === LeagueJobStatus.COMPLETED) {
          setGenBtnTxt('View Timetable');
        }
      } catch (err) {
        // Silently handle error for initial load
        if ((err as any)?.response?.status !== 404) {
          console.error('Failed to fetch job status:', err);
        }
      }
    };

    fetchJobStatus();
  }, [leagueId]);

  const handleGenerateLeague = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await api.get<LeagueJobStatusResponse>(
        `/timetable/job-status/${leagueId}`
      );

      const status = response.data;
      setJobStatus(status);

      // Navigate based on status
      if (status.status === LeagueJobStatus.COMPLETED) {
        navigate(`/league/timetable/${leagueId}`);
      } else if (
        status.status === LeagueJobStatus.CREATED ||
        status.status === LeagueJobStatus.PREPARED ||
        status.status === LeagueJobStatus.GENERATING
      ) {
        navigate(`/league/timetable-job/${leagueId}`);
      }
    } catch (err) {
      if ((err as any)?.response?.status === 404) {
        // Not found - redirect to job status page
        navigate(`/league/timetable-job/${leagueId}`);
      } else {
        setError(
          err instanceof Error ? err.message : 'Failed to fetch job status'
        );
      }
    } finally {
      setLoading(false);
    }
  };

  // Determine button color based on status
  const getButtonColor = (): string => {
    if (!jobStatus) {
      return '#4CAF50'; // Green (default)
    }

    if (
      jobStatus.status === LeagueJobStatus.FAILED ||
      jobStatus.status === LeagueJobStatus.CREATED
    ) {
      return '#4CAF50'; // Green
    }

    return '#FFD700'; // Yellow
  };

  const buttonStyle: React.CSSProperties = {
    display: 'block',
    margin: '20px auto',
    padding: '10px 24px',
    backgroundColor: getButtonColor(),
    color: '#000',
    border: 'none',
    borderRadius: '4px',
    fontSize: '16px',
    fontWeight: 'bold',
    cursor: loading ? 'not-allowed' : 'pointer',
    opacity: loading ? 0.7 : 1,
    transition: 'all 0.3s ease',
    textAlign: 'center' as const,
  };

  return (
    <div>
      <button
        onClick={handleGenerateLeague}
        disabled={loading}
        style={buttonStyle}
        onMouseEnter={(e) => {
          if (!loading) {
            (e.target as HTMLButtonElement).style.transform = 'scale(1.05)';
          }
        }}
        onMouseLeave={(e) => {
          (e.target as HTMLButtonElement).style.transform = 'scale(1)';
        }}
      >
        {loading ? 'Loading...' : genBtnTxt}
      </button>

      {error && (
        <div style={{ color: 'red', textAlign: 'center', marginTop: '10px' }}>
          {error}
        </div>
      )}
    </div>
  );
};

