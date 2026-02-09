import { Link } from "react-router-dom";
import CardsIcon from "@/assets/icons/CardsIcon";
import UsersIcon from "@/assets/icons/UsersIcon";
import DecksIcon from "@/assets/icons/DecksIcon";
import EffectsIcon from "@/assets/icons/EffectsIcon";
import GameRoomsIcon from "@/assets/icons/GameRoomsIcon";
import GamesIcon from "@/assets/icons/GamesIcon";
import MessagesIcon from "@/assets/icons/MessagesIcon";

const links = [
  {
    url: "/admin/users",
    label: "Users",
    icon: UsersIcon,
  },
  {
    url: "/admin/cards",
    label: "Cards",
    icon: CardsIcon,
  },
  {
    url: "/admin/decks",
    label: "Decks",
    icon: DecksIcon,
  },
  {
    url: "/admin/effects",
    label: "Effects",
    icon: EffectsIcon,
  },
  {
    url: "/admin/game-rooms",
    label: "Game Rooms",
    icon: GameRoomsIcon,
  },
  {
    url: "/admin/games",
    label: "Games",
    icon: GamesIcon,
  },
  {
    url: "/admin/messages",
    label: "Messages",
    icon: MessagesIcon,
  },
];

const AdminDashboard = () => {
  return (
    <div className="w-full flex-1 flex justify-center items-center flex-wrap content-center gap-10">
      {links.map((link, i) => (
        <Link
          to={link.url}
          key={`link-${i}`}
          className="bg-indigo-900/40 rounded-2xl w-[20%] h-60 flex flex-col justify-center items-center text-white/60 text-xl
        hover:text-white hover:bg-indigo-900/60 transition-colors duration-300 hover:[box-shadow:0_0_0.3rem_#bb00ff]"
        >
          <link.icon className="h-35" />
          {link.label}
        </Link>
      ))}
    </div>
  );
};

export default AdminDashboard;
