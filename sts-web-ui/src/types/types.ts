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