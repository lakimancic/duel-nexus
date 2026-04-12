import { createBrowserRouter } from "react-router-dom";
import { RootLayout } from "@/shared/layouts/RootLayout";
import { ProtectedLayout } from "@/shared/layouts/ProtectedLayout";
import { PublicOnlyLayout } from "@/shared/layouts/PublicOnlyLayout";
import WelcomePage from "@/features/auth/pages/Welcome";
import LoginPage from "@/features/auth/pages/Login";
import RegisterPage from "@/features/auth/pages/Register";
import { AdminLayout } from "@/shared/layouts/AdminLayout";
import AdminDashboard from "@/features/admin/pages/AdminDashboard";
import AdminCards from "@/features/admin/pages/AdminCards";
import LobbyPage from "@/features/lobby/pages/Lobby";
import PrivateMessagesPage from "@/features/lobby/pages/PrivateMessages";
import FriendlyPage from "@/features/friendly/pages/Friendly";
import GameRoomPage from "@/features/friendly/pages/GameRoom";
import DeckEditorPage from "@/features/decks/pages/DeckEditor";
import ProfilePage from "@/features/profile/pages/Profile";
import ScoreboardPage from "@/features/scoreboard/pages/Scoreboard";
import GamePage from "@/features/game/pages/Game";
import GameEndPage from "@/features/game/pages/GameEnd";
import DebugGamePage from "@/features/game/pages/DebugGame";

export const router = createBrowserRouter([
  {
    element: <RootLayout />,
    children: [
      {
        element: <PublicOnlyLayout />,
        children: [
          { path: "/", element: <WelcomePage /> },
          { path: "/login", element: <LoginPage /> },
          { path: "/register", element: <RegisterPage /> },
        ],
      },
      { path: "/debug", element: <DebugGamePage /> },
      {
        element: <ProtectedLayout />,
        children: [
          { path: "/lobby", element: <LobbyPage /> },
          { path: "/messages/private", element: <PrivateMessagesPage /> },
          { path: "/decks", element: <DeckEditorPage /> },
          { path: "/profile", element: <ProfilePage /> },
          { path: "/scoreboard", element: <ScoreboardPage /> },
          { path: "/friendly", element: <FriendlyPage /> },
          { path: "/game-room/:roomId", element: <GameRoomPage /> },
          { path: "/game/:gameId", element: <GamePage /> },
          { path: "/game/:gameId/end", element: <GameEndPage /> },
        ],
      },
      {
        element: <AdminLayout />,
        path: "/admin",
        children: [
          { index: true, element: <AdminDashboard /> },
          { path: "cards", element: <AdminCards /> }
        ]
      }
    ],
  },
]);
