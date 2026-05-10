import React from 'react';
import { Link, useParams } from 'react-router-dom';
import api from '../lib/api';
import {
  FullTimeTableMatch,
  FullTimeTableResponse,
  LeagueStadiumSummary,
  LeagueTeamSummary,
} from '../types/types';

function formatDate(value?: string): string {
  if (!value) return '-';

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return '-';

  return new Intl.DateTimeFormat(undefined, {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  }).format(date);
}

function formatDateTime(value?: string): string {
  if (!value) return '-';

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return '-';

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(date);
}

function formatTimeRange(match: FullTimeTableMatch): string {
  const startTime = match.timeSlot?.startTime;
  const endTime = match.timeSlot?.endTime;

  if (startTime && endTime) {
    return `${startTime} - ${endTime}`;
  }

  return formatDateTime(match.date);
}

function getInitials(name?: string): string {
  if (!name) return '?';

  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  return parts.slice(0, 2).map((part) => part[0]?.toUpperCase() ?? '').join('');
}

function getTeamLogo(team?: LeagueTeamSummary | null): string | undefined {
  return team?.logoUrl || undefined;
}

function getStadiumLogo(stadium?: LeagueStadiumSummary | null): string | undefined {
  return stadium?.logo || undefined;
}

function LogoBadge({
  name,
  logoUrl,
  className,
}: {
  name?: string;
  logoUrl?: string;
  className?: string;
}): React.ReactElement {
  if (logoUrl) {
    return <img className={className} src={logoUrl} alt={name ? `${name} logo` : 'logo'} />;
  }

  return <span className={className}>{getInitials(name)}</span>;
}

export default function LeagueTimetable(): React.ReactElement {
  const { leagueId } = useParams<{ leagueId: string }>();

  const [timetable, setTimetable] = React.useState<FullTimeTableResponse | null>(null);
  const [loading, setLoading] = React.useState(true);
  const [notFound, setNotFound] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const matches = React.useMemo(
    () => [...(timetable?.matches ?? [])].sort((left, right) => {
      const leftDate = new Date(left.date).getTime();
      const rightDate = new Date(right.date).getTime();

      if (!Number.isNaN(leftDate) && !Number.isNaN(rightDate) && leftDate !== rightDate) {
        return leftDate - rightDate;
      }

      const leftTime = left.timeSlot?.startTime || '';
      const rightTime = right.timeSlot?.startTime || '';

      return leftTime.localeCompare(rightTime);
    }),
    [timetable]
  );

  React.useEffect(() => {
    if (!leagueId) {
      setLoading(false);
      setError('Missing league id.');
      return;
    }

    let mounted = true;

    const loadTimetable = async () => {
      try {
        setLoading(true);
        setError(null);
        setNotFound(false);

        const response = await api.get<FullTimeTableResponse>(`/Timetable/full-league/${leagueId}`);
        if (!mounted) return;

        setTimetable(response.data);
      } catch (err: any) {
        if (!mounted) return;

        if (err?.response?.status === 404) {
          setNotFound(true);
          setTimetable(null);
          return;
        }

        setError('Failed to load timetable.');
        setTimetable(null);
      } finally {
        if (mounted) {
          setLoading(false);
        }
      }
    };

    void loadTimetable();

    return () => {
      mounted = false;
    };
  }, [leagueId]);

  if (!leagueId) {
    return (
      <section className="timetable-page timetable-page--empty" aria-live="polite">
        <div className="timetable-empty-card">
          <h2>Missing league id</h2>
          <p>The timetable page needs a league id in the route.</p>
        </div>
      </section>
    );
  }

  if (loading) {
    return (
      <section className="timetable-page" aria-live="polite" aria-busy="true">
        <div className="timetable-shell">
          <div className="timetable-loading-card">
            <div className="timetable-skeleton timetable-skeleton-title" />
            <div className="timetable-skeleton timetable-skeleton-subtitle" />
            <div className="timetable-skeleton timetable-skeleton-row" />
            <div className="timetable-skeleton timetable-skeleton-row" />
            <div className="timetable-skeleton timetable-skeleton-row" />
          </div>
        </div>
      </section>
    );
  }

  if (notFound) {
    return (
      <section className="timetable-page timetable-page--empty" aria-live="polite">
        <div className="timetable-empty-card">
          <p className="timetable-empty-label">Not fond</p>
          <h2>League timetable unavailable</h2>
          <p>This league has not produced a timetable yet, or the league could not be found.</p>
          <Link to="/" className="timetable-back-link">
            Back to home
          </Link>
        </div>
      </section>
    );
  }

  if (error) {
    return (
      <section className="timetable-page timetable-page--empty" aria-live="polite">
        <div className="timetable-empty-card">
          <p className="timetable-empty-label">Error</p>
          <h2>{error}</h2>
          <p>Try reloading the page.</p>
        </div>
      </section>
    );
  }

  const league = timetable?.league;

  return (
    <section className="timetable-page" aria-live="polite">
      <div className="timetable-shell">
        <header className="timetable-hero">
          <div className="timetable-hero-branding">
            <div className="timetable-league-logo">
              <LogoBadge name={league?.name} logoUrl={league?.logoUrl || undefined} className="timetable-league-logo-badge" />
            </div>
            <div>
              <p className="timetable-kicker">Full timetable</p>
              <h1>{league?.name || 'League timetable'}</h1>
              <p className="timetable-hero-copy">
                Generated {formatDateTime(timetable?.generatedAt)} with fitness score{' '}
                {typeof timetable?.bestFitness === 'number' ? timetable.bestFitness.toFixed(2) : '-'}
              </p>
            </div>
          </div>

          <div className="timetable-hero-meta">
            <div>
              <span>League ID</span>
              <strong>{leagueId}</strong>
            </div>
            <div>
              <span>Matches</span>
              <strong>{matches.length}</strong>
            </div>
            <div>
              <span>Start date</span>
              <strong>{formatDate(league?.startDate)}</strong>
            </div>
          </div>
        </header>

        {matches.length === 0 ? (
          <div className="timetable-empty-card timetable-empty-card--inline">
            <p className="timetable-empty-label">No matches</p>
            <h2>The timetable is empty.</h2>
            <p>Once matches are generated, they will appear here.</p>
          </div>
        ) : (
          <div className="timetable-table-wrap">
            <table className="timetable-table">
              <thead>
                <tr>
                  <th scope="col">Time</th>
                  <th scope="col">Match</th>
                  <th scope="col">Stadium</th>
                  <th scope="col">Date</th>
                </tr>
              </thead>
              <tbody>
                {matches.map((match) => {
                  const team1 = match.team1;
                  const team2 = match.team2;
                  const stadium = match.stadium;

                  return (
                    <tr key={match.id}>
                      <td className="timetable-cell-time">
                        <div className="timetable-time-range">{formatTimeRange(match)}</div>
                        <div className="timetable-time-id">Match #{match.id.slice(0, 8)}</div>
                      </td>

                      <td>
                        <div className="timetable-matchup">
                          <div className="timetable-team-card timetable-team-card--left">
                            <LogoBadge
                              name={team1?.name}
                              logoUrl={getTeamLogo(team1)}
                              className="timetable-team-logo"
                            />
                            <span className="timetable-team-name">{team1?.name || 'TBD'}</span>
                          </div>

                          <span className="timetable-vs">vs</span>

                          <div className="timetable-team-card timetable-team-card--right">
                            <LogoBadge
                              name={team2?.name}
                              logoUrl={getTeamLogo(team2)}
                              className="timetable-team-logo"
                            />
                            <span className="timetable-team-name">{team2?.name || 'TBD'}</span>
                          </div>
                        </div>
                      </td>

                      <td>
                        <div className="timetable-stadium-chip">
                          <LogoBadge
                            name={stadium?.name}
                            logoUrl={getStadiumLogo(stadium)}
                            className="timetable-stadium-logo"
                          />
                          <span>{stadium?.name || 'TBD'}</span>
                        </div>
                      </td>

                      <td className="timetable-cell-date">{formatDate(match.date)}</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </section>
  );
}
