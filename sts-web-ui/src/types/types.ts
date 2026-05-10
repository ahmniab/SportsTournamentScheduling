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