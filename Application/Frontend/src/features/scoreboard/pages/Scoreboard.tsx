import { AxiosError } from "axios";
import { useEffect, useState } from "react";
import { FaCrown, FaMedal, FaTrophy } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import type { ErrorMessage } from "@/shared/types/error.types";
import { scoreboardApi } from "../api/scoreboard.api";
import type { ScoreboardEntryDto } from "../types/scoreboard.types";

const rankBadgeClass = (rank: number) => {
  if (rank === 1) return "bg-amber-300/30 text-amber-100 border-amber-200/55";
  if (rank === 2) return "bg-slate-300/25 text-slate-100 border-slate-200/50";
  if (rank === 3) return "bg-orange-400/25 text-orange-100 border-orange-200/50";
  return "bg-white/10 text-white/80 border-white/20";
};

const rankIcon = (rank: number) => {
  if (rank === 1) return <FaCrown className="size-4" />;
  if (rank <= 3) return <FaMedal className="size-4" />;
  return null;
};

const ScoreboardPage = () => {
  const navigate = useNavigate();
  const [entries, setEntries] = useState<ScoreboardEntryDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let disposed = false;

    const loadScoreboard = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const response = await scoreboardApi.getScoreboard();
        if (!disposed) {
          setEntries(response.data);
        }
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setError(data?.error ?? "Failed to load scoreboard.");
        }
      } finally {
        if (!disposed) {
          setIsLoading(false);
        }
      }
    };

    void loadScoreboard();

    return () => {
      disposed = true;
    };
  }, []);

  return (
    <div className="h-screen w-full p-4 md:p-6">
      <div className="mx-auto h-full max-w-4xl rounded-xl border border-white/20 bg-[#091b33]/82 p-4 md:p-5 text-white">
        <div className="mb-4 flex items-center justify-between border-b border-white/20 pb-3">
          <h1 className="inline-flex items-center gap-2 text-xl font-bold text-amber-100">
            <FaTrophy className="size-5 text-amber-300" />
            Ranked Scoreboard
          </h1>
          <button
            type="button"
            onClick={() => navigate("/lobby")}
            className="rounded-md border border-amber-300/40 bg-[#4b1812]/70 px-3 py-1.5 text-sm text-amber-100 hover:text-white hover:border-amber-200 transition-colors cursor-pointer"
          >
            Back to lobby
          </button>
        </div>

        <div className="h-[calc(100%-4.25rem)] overflow-y-auto pr-1">
          {isLoading && <p className="text-sm text-white/70">Loading scoreboard...</p>}
          {!isLoading && error && <p className="text-sm text-red-300">{error}</p>}

          {!isLoading && !error && entries.length === 0 && (
            <p className="text-sm text-white/70">No ranked players yet.</p>
          )}

          {!isLoading && !error && entries.length > 0 && (
            <div className="space-y-2">
              {entries.map((entry) => (
                <div
                  key={entry.userId}
                  className="flex items-center justify-between rounded-lg border border-white/20 bg-[#4b1812]/55 px-4 py-3"
                >
                  <div className="flex items-center gap-3">
                    <span className={`inline-flex items-center gap-2 rounded-md border px-2 py-1 text-xs font-bold ${rankBadgeClass(entry.rank)}`}>
                      {rankIcon(entry.rank)}
                      #{entry.rank}
                    </span>
                    <p className="font-semibold text-white">{entry.username}</p>
                  </div>
                  <p className="text-sm font-bold text-emerald-200">Elo {entry.elo.toFixed(2)}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ScoreboardPage;
