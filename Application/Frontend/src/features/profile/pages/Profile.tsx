import { AxiosError } from "axios";
import { useEffect, useMemo, useState } from "react";
import { FaChartLine, FaLayerGroup, FaPlay } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import DeckEditorPanel from "@/features/decks/components/DeckEditorPanel";
import type { ErrorMessage } from "@/shared/types/error.types";
import { profileApi } from "../api/profile.api";
import type {
  UserProfileGameDto,
  UserProfileGamePlayerDto,
  UserProfileStatsDto,
} from "../types/profile.types";

type ProfileTab = "stats" | "decks";

const formatDateTime = (value: string | null) => {
  if (!value) return "In progress";
  const parsedDate = new Date(value);
  if (Number.isNaN(parsedDate.getTime())) return "Unknown";
  return parsedDate.toLocaleString();
};

const PlayerRatingLine = ({ player }: { player: UserProfileGamePlayerDto }) => {
  return (
    <div
      className={`rounded-md border px-3 py-2 text-sm ${
        player.isWinner
          ? "border-amber-200/55 bg-amber-500/12"
          : "border-white/15 bg-white/5"
      }`}
    >
      <div className="flex items-center justify-between gap-3">
        <p className="font-semibold text-white">
          #{player.placement} {player.username}
        </p>
        <p className="text-xs text-white/70">LP: {player.lifePoints}</p>
      </div>

      <div className="mt-1 flex items-center justify-between gap-3">
        <p className="text-xs text-white/70">Rating: {(player.newElo ?? player.oldElo ?? 0).toFixed(2)}</p>
      </div>
    </div>
  );
};

const GameCard = ({ game, onContinue }: { game: UserProfileGameDto; onContinue: (gameId: string) => void }) => {
  const isInProgress = !game.finishedAt;

  return (
    <div className="rounded-lg border border-white/20 bg-black/25 p-4">
      <div className="flex items-center justify-between gap-3">
        <div>
          <p className="text-sm text-amber-200 font-semibold">
            {game.isRanked ? "Ranked" : "Friendly"} game
          </p>
          <p className="text-xs text-white/65">
            Started: {formatDateTime(game.startedAt)}
          </p>
          <p className="text-xs text-white/65">
            Finished: {formatDateTime(game.finishedAt)}
          </p>
        </div>

        <div className="flex items-center gap-2">
          <span
            className={`rounded-md px-2 py-1 text-xs font-bold ${
              game.placement === 1
                ? "bg-amber-500/25 text-amber-100"
                : "bg-slate-400/20 text-slate-200"
            }`}
          >
            Place #{game.placement ?? "-"}
          </span>

          {isInProgress && (
            <button
              type="button"
              onClick={() => onContinue(game.gameId)}
              className="inline-flex items-center gap-2 rounded-md border border-emerald-300/40 bg-emerald-500/20 px-3 py-1.5 text-xs font-semibold text-emerald-100 hover:bg-emerald-500/30 cursor-pointer"
            >
              <FaPlay className="size-3" />
              Continue
            </button>
          )}
        </div>
      </div>

      <div className="mt-3 space-y-2">
        {game.players.map((player) => (
          <PlayerRatingLine key={player.playerGameId} player={player} />
        ))}
      </div>
    </div>
  );
};

const ProfilePage = () => {
  const navigate = useNavigate();
  const [tab, setTab] = useState<ProfileTab>("stats");
  const [stats, setStats] = useState<UserProfileStatsDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let disposed = false;

    const loadProfile = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const response = await profileApi.getMyProfileStats();
        if (!disposed) {
          setStats(response.data);
        }
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setError(data?.error ?? "Failed to load profile stats.");
        }
      } finally {
        if (!disposed) {
          setIsLoading(false);
        }
      }
    };

    void loadProfile();

    return () => {
      disposed = true;
    };
  }, []);

  const sortedGames = useMemo(
    () => [...(stats?.games ?? [])].sort((left, right) => +new Date(right.startedAt) - +new Date(left.startedAt)),
    [stats?.games]
  );

  return (
    <div className="h-screen w-full p-4 md:p-6">
      <div className="mx-auto h-full max-w-7xl rounded-xl border border-white/20 bg-[#091b33]/82 p-4 md:p-5 text-white">
        <div className="mb-3 flex items-center justify-between border-b border-white/20 pb-3">
          <h1 className="text-xl font-bold text-white">Profile</h1>
          <button
            type="button"
            onClick={() => navigate("/lobby")}
            className="rounded-md border border-amber-300/40 bg-[#4b1812]/70 px-3 py-1.5 text-sm text-amber-100 hover:text-white hover:border-amber-200 transition-colors cursor-pointer"
          >
            Back to lobby
          </button>
        </div>

        <div className="mb-4 flex overflow-hidden rounded-lg border border-white/20 bg-[#4b1812]/55">
          <button
            type="button"
            onClick={() => setTab("stats")}
            className={`flex-1 inline-flex items-center justify-center gap-2 px-3 py-2 text-sm font-semibold cursor-pointer ${
              tab === "stats" ? "bg-amber-500/25 text-amber-100" : "text-white/70 hover:text-white"
            }`}
          >
            <FaChartLine className="size-4" />
            User Stats
          </button>
          <button
            type="button"
            onClick={() => setTab("decks")}
            className={`flex-1 inline-flex items-center justify-center gap-2 px-3 py-2 text-sm font-semibold cursor-pointer ${
              tab === "decks" ? "bg-amber-500/25 text-amber-100" : "text-white/70 hover:text-white"
            }`}
          >
            <FaLayerGroup className="size-4" />
            Deck Editor
          </button>
        </div>

        <div className="h-[calc(100%-8.6rem)]">
          {tab === "stats" && (
            <div className="h-full overflow-y-auto pr-1">
              {isLoading && <p className="text-sm text-white/70">Loading profile stats...</p>}
              {!isLoading && error && <p className="text-sm text-red-300">{error}</p>}

              {!isLoading && !error && stats && (
                <div className="space-y-4">
                  <div className="rounded-lg border border-amber-300/35 bg-[#4b1812]/70 p-5 text-center">
                    <p className="text-sm uppercase tracking-[0.08em] text-amber-100/70">Player</p>
                    <p className="mt-1 text-3xl font-extrabold text-amber-100">{stats.username}</p>
                    <p className="mt-2 text-lg font-semibold text-emerald-200">Elo {stats.elo.toFixed(2)}</p>
                  </div>

                  <div className="rounded-lg border border-white/20 bg-[#4b1812]/55 p-4">
                    <p className="mb-3 text-lg font-semibold text-amber-200">Current and Previous Games</p>
                    {sortedGames.length === 0 ? (
                      <p className="text-sm text-white/70">No games played yet.</p>
                    ) : (
                      <div className="space-y-3">
                        {sortedGames.map((game) => (
                          <GameCard key={game.gameId} game={game} onContinue={(gameId) => navigate(`/game/${gameId}`)} />
                        ))}
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>
          )}

          {tab === "decks" && <DeckEditorPanel showBackToLobby={false} />}
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
