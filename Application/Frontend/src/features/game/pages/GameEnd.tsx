import { AxiosError } from "axios";
import { useEffect, useMemo, useState } from "react";
import { FaArrowDown, FaArrowUp } from "react-icons/fa";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import Logo from "@/assets/images/logo.png";
import { gameApi } from "@/features/game/api/game.api";
import type { GameResultDto } from "@/features/game/types/game.types";
import type { ErrorMessage } from "@/shared/types/error.types";

const GameEndPage = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();

  const initialResult = (location.state as { result?: GameResultDto } | null)?.result ?? null;

  const [result, setResult] = useState<GameResultDto | null>(initialResult);
  const [isLoading, setIsLoading] = useState(!initialResult);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!gameId) {
      navigate("/lobby", { replace: true });
      return;
    }

    if (initialResult) return;

    let disposed = false;

    const loadResult = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await gameApi.getGameResult(gameId);
        if (!disposed) {
          setResult(response.data);
        }
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setError(data?.error ?? "Failed to load game result.");
        }
      } finally {
        if (!disposed) {
          setIsLoading(false);
        }
      }
    };

    void loadResult();

    return () => {
      disposed = true;
    };
  }, [gameId, initialResult, navigate]);

  const sortedChanges = useMemo(() => {
    return [...(result?.playerRatingChanges ?? [])].sort((left, right) => {
      if (left.isWinner && !right.isWinner) return -1;
      if (!left.isWinner && right.isWinner) return 1;
      return right.delta - left.delta;
    });
  }, [result?.playerRatingChanges]);

  return (
    <main className="min-h-screen w-full flex items-center justify-center px-4 py-8">
      <div className="w-full max-w-3xl rounded-xl bg-[#4b1812]/85 border border-amber-300/20 p-6 text-white">
        <img src={Logo} alt="logo" className="w-[55%] mx-auto" />

        {isLoading && <p className="mt-6 text-center text-white/80">Loading game result...</p>}

        {!isLoading && error && <p className="mt-6 text-center text-red-300">{error}</p>}

        {!isLoading && !error && result && (
          <div className="mt-6 space-y-4">
            <div className="rounded-lg border border-white/20 bg-black/25 p-4 text-center">
              <p className="text-sm text-white/70 uppercase tracking-[0.08em]">Winner</p>
              <p className="mt-1 text-2xl font-bold text-amber-200">
                {result.winnerUsername ?? "No winner"}
              </p>
              <p className="mt-2 text-xs text-white/60">
                {result.isRanked ? "Ranked match" : "Friendly match"}
              </p>
            </div>

            <div className="rounded-lg border border-white/20 bg-black/25 p-4">
              <p className="text-amber-200 font-semibold mb-3">Rating changes</p>
              <div className="space-y-2">
                {sortedChanges.map((change) => {
                  const isPositive = change.delta > 0;
                  const isNegative = change.delta < 0;

                  return (
                    <div
                      key={change.playerGameId}
                      className="flex items-center justify-between rounded-md border border-white/15 bg-white/5 px-3 py-2"
                    >
                      <p className="font-semibold">
                        {change.username}
                        {change.isWinner ? " (Winner)" : ""}
                      </p>
                      <div className="flex items-center gap-3">
                        <p className="text-xs text-white/70">
                          {change.oldElo.toFixed(2)} → {change.newElo.toFixed(2)}
                        </p>
                        <span
                          className={`flex items-center gap-1 font-bold ${
                            isPositive
                              ? "text-green-300"
                              : isNegative
                                ? "text-red-300"
                                : "text-white/70"
                          }`}
                        >
                          {isPositive && <FaArrowUp className="size-3" />}
                          {isNegative && <FaArrowDown className="size-3" />}
                          {change.delta > 0 ? `+${change.delta.toFixed(2)}` : change.delta.toFixed(2)}
                        </span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            <div className="flex justify-center gap-3 pt-2">
              <button
                type="button"
                onClick={() => navigate("/lobby", { replace: true })}
                className="rounded-md border border-white/25 bg-white/10 px-4 py-2 font-semibold hover:bg-white/15 cursor-pointer"
              >
                Back to Lobby
              </button>
              <button
                type="button"
                onClick={() => navigate("/friendly", { replace: true })}
                className="rounded-md border border-amber-300/30 bg-amber-500/20 px-4 py-2 font-semibold hover:bg-amber-500/30 cursor-pointer"
              >
                Play Again
              </button>
            </div>
          </div>
        )}
      </div>
    </main>
  );
};

export default GameEndPage;
