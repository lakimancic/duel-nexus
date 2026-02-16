import { AxiosError } from "axios";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Logo from "@/assets/images/logo.png";
import ButtonBackground from "@/assets/images/btnBackground.png";
import type { ErrorMessage } from "@/shared/types/error.types";
import { friendlyApi } from "../api/friendly.api";
import type { FriendlyNavigationState } from "../types/friendly.types";

const FriendlyPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [joinCode, setJoinCode] = useState("");
  const [isCreating, setIsCreating] = useState(false);
  const [isJoining, setIsJoining] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const state = location.state as FriendlyNavigationState | null;
    if (state?.message) {
      setError(state.message);
      navigate(location.pathname, { replace: true, state: null });
    }
  }, [location.pathname, location.state, navigate]);

  const handleCreateRoom = async () => {
    setError(null);
    setIsCreating(true);

    try {
      const response = await friendlyApi.createFriendlyRoom();
      navigate(`/game-room/${response.data.id}`);
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to create room.");
      }
    } finally {
      setIsCreating(false);
    }
  };

  const handleJoinRoom = async () => {
    const normalizedCode = joinCode.trim().toUpperCase();
    if (!normalizedCode) {
      setError("Join code is required.");
      return;
    }

    setError(null);
    setIsJoining(true);

    try {
      const response = await friendlyApi.joinFriendlyRoom(normalizedCode);
      navigate(`/game-room/${response.data.id}`);
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to join room.");
      }
    } finally {
      setIsJoining(false);
    }
  };

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="w-full max-w-4xl flex flex-col items-center gap-4 px-4">
        <img src={Logo} alt="logo" className="w-[min(92vw,36rem)]" />

        <div className="w-full max-w-xl rounded-xl bg-[#4b1812]/80 p-6 border border-amber-300/20 text-white">
          <div className="flex flex-col items-center gap-3">
            <p className="text-amber-200 text-xl font-bold [text-shadow:0_0_0.8rem_#e26807]">
              Friendly Games
            </p>

            {error && <p className="text-sm text-red-300 text-center">{error}</p>}

            <button
              type="button"
              disabled={isCreating || isJoining}
              onClick={() => void handleCreateRoom()}
              className="relative cursor-pointer group disabled:opacity-60 disabled:cursor-default"
            >
              <img src={ButtonBackground} alt="button-bg" className="h-32" />
              <p
                className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-xl font-bold w-full
                [text-shadow:0_0_1rem_#5a3485] group-hover:text-2xl transition-all duration-500"
              >
                {isCreating ? "CREATING..." : "CREATE MATCH"}
              </p>
            </button>

            <div className="w-full max-w-sm flex flex-col gap-2 mt-2">
              <label htmlFor="join-code" className="text-sm text-center text-amber-100/80">
                Join code
              </label>
              <input
                id="join-code"
                value={joinCode}
                onChange={(event) => setJoinCode(event.target.value.toUpperCase())}
                maxLength={6}
                placeholder="ENTER CODE"
                className="w-full rounded-md border border-amber-300/30 bg-black/20 px-3 py-2 text-center tracking-[0.25em]
                text-amber-100 placeholder:text-amber-100/40 outline-none focus:border-amber-300"
              />
            </div>

            <button
              type="button"
              disabled={isJoining || isCreating}
              onClick={() => void handleJoinRoom()}
              className="relative cursor-pointer group disabled:opacity-60 disabled:cursor-default"
            >
              <img src={ButtonBackground} alt="button-bg" className="h-32" />
              <p
                className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-xl font-bold w-full
                [text-shadow:0_0_1rem_#5a3485] group-hover:text-2xl transition-all duration-500"
              >
                {isJoining ? "JOINING..." : "JOIN MATCH"}
              </p>
            </button>

            <button
              type="button"
              onClick={() => navigate("/lobby")}
              className="mt-1 text-amber-100/80 hover:text-amber-100 cursor-pointer"
            >
              Back to lobby
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default FriendlyPage;
