import Chat from "@/shared/components/Chat";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaComments, FaUserCircle } from "react-icons/fa";
import Logo from "@/assets/images/logo.png";
import ButtonBackground from "@/assets/images/btnBackground.png";
import { useGlobalChat } from "../hooks/useGlobalChat";
import { useActiveUsers } from "../hooks/useActiveUsers";
import ActiveUsers from "../components/ActiveUsers";
import { usePrivateMessagesStore } from "../store/private-messages.store";
import { gameHub } from "@/shared/realtime/gameHub";
import { useAuthStore } from "@/features/auth/store/auth.store";

type TabValues = "global" | "players";
const tabValues = [
  { value: "global", label: "Global Chat" },
  { value: "players", label: "Active Players" },
];

const LobbyPage = () => {
  const navigate = useNavigate();
  const [tab, setTab] = useState<TabValues>("global");
  const myUserId = useAuthStore((state) => state.userId);
  const unreadCount = usePrivateMessagesStore((state) => state.unreadCount);
  const incrementUnread = usePrivateMessagesStore((state) => state.incrementUnread);
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

  const openPrivateMessagesForUser = (user: { id: string; username: string }) => {
    if (!myUserId || user.id === myUserId) return;
    navigate("/messages/private", {
      state: {
        targetUserId: user.id,
        targetUsername: user.username,
      },
    });
  };

  useEffect(() => {
    const privateMessageHandler = (message: { senderId: string }) => {
      if (!myUserId) return;
      if (message.senderId === myUserId) return;
      incrementUnread();
    };

    gameHub.onPrivateChat(privateMessageHandler);
    return () => {
      gameHub.offPrivateChat(privateMessageHandler);
    };
  }, [incrementUnread, myUserId]);

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="absolute top-4 right-5 z-10 flex items-center gap-3">
        <button
          type="button"
          onClick={() => navigate("/decks")}
          className="rounded-full border border-amber-300/50 bg-[#4b1812]/75 p-2 text-amber-200 hover:text-white hover:border-amber-200 transition-colors cursor-pointer"
          title="Deck Editor"
        >
          <FaUserCircle className="size-6" />
        </button>
        <button
          type="button"
          onClick={() => navigate("/messages/private")}
          className="relative rounded-full border border-white/30 bg-[#091b33]/70 p-2 text-white hover:text-amber-100 hover:border-amber-200 transition-colors cursor-pointer"
          title="Private Messages"
        >
          <FaComments className="size-6" />
          {unreadCount > 0 && (
            <span className="absolute -top-1 -right-1 min-w-5 h-5 px-1 rounded-full bg-red-600 text-white text-[11px] font-bold flex items-center justify-center">
              {unreadCount}
            </span>
          )}
        </button>
      </div>

      <div className="flex-5 flex flex-col justify-center items-center relative h-full">
        <img src={Logo} alt="logo" className="w-[75%]" />
        <div className="relative cursor-pointer group" onClick={() => {}}>
          <img src={ButtonBackground} alt="button-bg" className="h-45" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-2xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-3xl transition-all duration-500"
          >
            PLAY RANKED
          </p>
        </div>
        <div
          className="relative cursor-pointer group"
          onClick={() => navigate("/friendly")}
        >
          <img src={ButtonBackground} alt="button-bg" className="h-45" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-2xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-3xl transition-all duration-500"
          >
            PLAY FRIENDLY
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
                onSenderClick={openPrivateMessagesForUser}
                canSenderClick={(sender) => sender.id !== myUserId}
              />
            )}
            {tab === "players" && (
              <ActiveUsers
                users={activeUsers}
                isLoading={isLoadingActiveUsers}
                currentUserId={myUserId}
                onUserClick={openPrivateMessagesForUser}
              />
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default LobbyPage;
