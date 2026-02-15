import Chat from "@/shared/components/Chat";
import { useState } from "react";
import Logo from "@/assets/images/logo.png";
import ButtonBackground from "@/assets/images/btnBackground.png";
import { useGlobalChat } from "../hooks/useGlobalChat";
import { useActiveUsers } from "../hooks/useActiveUsers";
import ActiveUsers from "../components/ActiveUsers";

type TabValues = "global" | "players";
const tabValues = [
  { value: "global", label: "Global Chat" },
  { value: "players", label: "Active Players" },
];

const LobbyPage = () => {
  const [tab, setTab] = useState<TabValues>("global");
  const {
    messages,
    isLoading,
    isLoadingMore,
    isSending,
    canSeeMore,
    onSeeMore,
    onSend,
  } = useGlobalChat();
  const { activeUsers, isLoading: isLoadingActiveUsers } = useActiveUsers();

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="flex-5 flex flex-col justify-center items-center relative h-full">
        <img src={Logo} alt="logo" className="w-[75%]" />
        <div className="relative cursor-pointer group" onClick={() => {}}>
          <img src={ButtonBackground} alt="button-bg" className="h-50" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-3xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-4xl transition-all duration-500"
          >
            PLAY RANKED
          </p>
        </div>
        <div className="relative cursor-pointer group" onClick={() => {}}>
          <img src={ButtonBackground} alt="button-bg" className="h-50" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-3xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-4xl transition-all duration-500"
          >
            CREATE FRIENDLY
          </p>
        </div>
      </div>
      <div className="flex-4 h-full flex flex-col justify-center items-center">
        <div
          className={`w-[90%] ${tab === "global" ? "bg-[#4b1812]/80" : "bg-[#091b33]/80"} min-h-[80%] rounded-xl`}
        >
          <div className="flex justify-center border-b border-white/30">
            {tabValues.map((tabValue, tabIndex) => (
              <div key={tabValue.value} className="contents">
                <button
                  type="button"
                  disabled={tab === tabValue.value}
                  className="flex-1 disabled:text-white text-white/50 hover:text-white/80 cursor-pointer py-3 text-md"
                  onClick={() => setTab(tabValue.value as TabValues)}
                >
                  {tabValue.label}
                </button>
                {tabIndex < tabValues.length - 1 && (
                  <div className="w-px bg-white/30"></div>
                )}
              </div>
            ))}
          </div>
          <div className="h-[calc(80vh-3.2rem)]">
            {tab === "global" && (
              <Chat
                messages={messages}
                isLoading={isLoading}
                isSending={isSending}
                isLoadingMore={isLoadingMore}
                canSeeMore={canSeeMore}
                onSeeMore={onSeeMore}
                onSend={onSend}
              />
            )}
            {tab === "players" && (
              <ActiveUsers
                users={activeUsers}
                isLoading={isLoadingActiveUsers}
              />
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default LobbyPage;
