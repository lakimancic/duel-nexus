import { Link, Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "@/features/auth/store/auth.store";
import Logo from "@/assets/images/logo.png";

export const AdminLayout = () => {
  const isAuthenticated = useAuthStore(state => state.isAuthenticated);
  const role = useAuthStore(state => state.role);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (role !== "Admin") {
    return <Navigate to="/lobby" replace />;
  }

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="h-full w-full backdrop-blur-xl absolute bg-indigo-950/80"></div>
      <div className="relative min-h-full h-screen w-full rounded-md flex flex-col">
        <header className="flex items-center">
          <Link to="/lobby">
            <img src={Logo} alt="logo" className="h-25 m-3" />
          </Link>
          <h1 className="text-3xl font-bold mb-5 text-purple-200 [text-shadow:0_0_0.8rem_#bb00ff]">
            Admin Dashboard
          </h1>
        </header>
        <Outlet />
      </div>
    </div>
  );
};
