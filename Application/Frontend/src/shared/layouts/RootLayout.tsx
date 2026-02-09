import { Outlet } from "react-router-dom";
import backgroundImage from "@/assets/images/background.png";

export const RootLayout = () => {
  return (
    <div
      className="min-h-screen bg-cover bg-center"
      style={{ backgroundImage: `url(${backgroundImage})` }}
    >
      <Outlet />
    </div>
  );
};
