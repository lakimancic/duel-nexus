import { useNavigate } from "react-router-dom";
import Logo from "@/assets/images/logo.png";
import ButtonBackground from "@/assets/images/btnBackground.png";

const WelcomePage = () => {
  const navigate = useNavigate();

  return (
    <div className="h-screen flex flex-col items-center justify-center gap-10">
      <img src={Logo} alt="logo" className="h-120" />

      <div className="grid grid-cols-2 gap-20">
        <div
          className="relative cursor-pointer group"
          onClick={() => navigate("/login")}
        >
          <img src={ButtonBackground} alt="button-bg" className="h-50" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-3xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-4xl transition-all duration-500"
          >
            LOGIN
          </p>
        </div>

        <div
          className="relative cursor-pointer group"
          onClick={() => navigate("/register")}
        >
          <img src={ButtonBackground} alt="button-bg" className="h-50" />
          <p
            className="absolute top-[50%] translate-y-[-40%] left-0 text-white text-center text-3xl font-bold w-full
            [text-shadow:0_0_1rem_#5a3485] group-hover:text-4xl transition-all duration-500"
          >
            REGISTER
          </p>
        </div>
      </div>
    </div>
  );
};

export default WelcomePage;
