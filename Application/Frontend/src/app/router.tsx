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
      {
        element: <ProtectedLayout />,
        children: [{ path: "/lobby", element: <LobbyPage /> }],
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
