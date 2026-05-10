export type League = {
  id: string;
  name: string;
  logoUrl?: string;
  startDate: string;
};

export type Team = {
  id: string;
  leagueId: string;
  name: string;
  logoUrl?: string;
  createdAt: string;
};

export type Stadium = {
  id: string;
  leagueId: string;
  name: string;
  // API sometimes returns `logo` or `logoUrl` — support both
  logo?: string;
  logoUrl?: string;
};

export type TimeSlot = {
  id: string;
  leagueId: string;
  startTime: string;
  endTime: string;
};

export type LeagueTeamSummary = {
  id: string;
  leagueId: string;
  name: string;
  logoUrl?: string | null;
};

export type LeagueStadiumSummary = {
  id: string;
  leagueId: string;
  name: string;
  logo?: string | null;
};

export type LeagueTimeSlotSummary = {
  id: string;
  leagueId: string;
  startTime: string;
  endTime: string;
};

export type LeagueSummary = {
  id: string;
  ownerId: string;
  name: string;
  createdAt: string;
  startDate: string;
  logoUrl?: string | null;
  teams?: LeagueTeamSummary[] | null;
  stadiums?: LeagueStadiumSummary[] | null;
  timeSlots?: LeagueTimeSlotSummary[] | null;
};

export type FullTimeTableMatch = {
  id: string;
  league?: LeagueSummary | null;
  team1?: LeagueTeamSummary | null;
  team2?: LeagueTeamSummary | null;
  timeSlot?: LeagueTimeSlotSummary | null;
  stadium?: LeagueStadiumSummary | null;
  date: string;
};

export type FullTimeTableResponse = {
  id: string;
  generatedAt: string;
  bestFitness: number;
  league?: LeagueSummary | null;
  matches: FullTimeTableMatch[];
};

export enum LeagueJobStatus {
  CREATED = 0,
  PREPARED = 1,
  GENERATING = 2,
  COMPLETED = 3,
  FAILED = 4,
}

export type ProtoTimestamp = {
  seconds: number | string;
  nanos?: number;
};

export type LeagueJobStatusResponse = {
  leagueId: string;
  status: LeagueJobStatus;
  createdAt?: string | ProtoTimestamp;
  startedAt?: string | ProtoTimestamp;
  errorMessage?: string;
};

