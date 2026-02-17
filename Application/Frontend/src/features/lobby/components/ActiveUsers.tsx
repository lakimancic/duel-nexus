import type { ShortUserDto } from "@/shared/types/user.types";

interface ActiveUsersProps {
  users: ShortUserDto[];
  isLoading: boolean;
  currentUserId?: string | null;
  onUserClick?: (user: ShortUserDto) => void;
}

const ActiveUsers = ({ users, isLoading, currentUserId, onUserClick }: ActiveUsersProps) => {
  return (
    <div className="h-full w-full text-white flex flex-col">
      <div className="px-4 pt-4 pb-2 text-sm text-white/70">
        {users.length} active {users.length === 1 ? "player" : "players"}
      </div>
      <div className="flex-1 overflow-y-auto px-4 pb-4 space-y-2">
        {isLoading && (
          <p className="text-center text-sm text-white/70 py-2">
            Loading active players...
          </p>
        )}
        {!isLoading && users.length === 0 && (
          <p className="text-center text-sm text-white/70 py-2">
            No active players right now.
          </p>
        )}
        {users.map((user) => (
          <button
            key={user.id}
            type="button"
            onClick={() => onUserClick?.(user)}
            disabled={!onUserClick || user.id === currentUserId}
            className="w-full rounded-lg border border-white/20 bg-black/20 px-3 py-2 flex items-center gap-3 text-left
            disabled:cursor-default disabled:opacity-80 enabled:hover:bg-black/30 enabled:cursor-pointer transition-colors"
          >
            <span className="h-2.5 w-2.5 rounded-full bg-green-400 shadow-[0_0_10px_rgba(74,222,128,0.8)]" />
            <span className="text-sm font-medium">{user.username}</span>
          </button>
        ))}
      </div>
    </div>
  );
};

export default ActiveUsers;
