export interface UserProfileGamePlayerDto {
  playerGameId: string;
  userId: string;
  username: string;
  placement: number;
  lifePoints: number;
  isWinner: boolean;
  oldElo: number | null;
  newElo: number | null;
  delta: number | null;
}

export interface UserProfileGameDto {
  gameId: string;
  startedAt: string;
  finishedAt: string | null;
  isRanked: boolean;
  placement: number | null;
  eloChangeAvailable: boolean;
  players: UserProfileGamePlayerDto[];
}

export interface UserProfileStatsDto {
  userId: string;
  username: string;
  elo: number;
  games: UserProfileGameDto[];
}
