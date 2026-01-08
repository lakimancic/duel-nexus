import { createBrowserRouter } from "react-router-dom";
import { RootLayout } from "../shared/layouts/RootLayout";
import { ProtectedLayout } from "../shared/layouts/ProtectedLayout";
import { PublicOnlyLayout } from "../shared/layouts/PublicOnlyLayout";
import WelcomePage from "../features/auth/pages/Welcome";
import LoginPage from "../features/auth/pages/Login";
import RegisterPage from "../features/auth/pages/Register";

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
        children: [{ path: "/lobby", element: <div>Lobby</div> }],
      },
    ],
  },
]);
