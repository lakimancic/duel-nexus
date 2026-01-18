import { useAuth } from "../hooks/useAuth";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { FaArrowRight } from "react-icons/fa";
import Logo from "../../../assets/images/logo.png";

const loginSchema = z.object({
  email: z.email().min(1, "Email is required"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

type LoginForm = z.infer<typeof loginSchema>;

const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginForm) => {
    try {
      await login(data);
      navigate("/lobby");
    } catch (err) {
      console.error(err);
      alert("Invalid credentials");
    }
  };

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="h-full w-full backdrop-blur-xl absolute"></div>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="relative flex flex-col gap-2 w-120 bg-[#4b1812]/80 text-white p-8 items-center pt-30 rounded-xl"
      >
        <img src={Logo} alt="logo" className="absolute -top-30 w-[90%]" />
        <h1 className="text-2xl font-bold mb-5 text-amber-200 [text-shadow:0_0_0.8rem_#e26807]">
          Log-In to your account
        </h1>
        <input
          {...register("email")}
          placeholder="Email"
          className="p-2 w-full rounded border border-amber-300 placeholder:text-amber-200/50 text-amber-300 outline-none"
        />
        {errors.email && (
          <span className="text-red-500 text-sm">{errors.email.message}</span>
        )}

        <input
          {...register("password")}
          type="password"
          placeholder="Password"
          className="p-2 mt-5 w-full rounded border border-amber-300 placeholder:text-amber-200/50 text-amber-300 outline-none"
        />
        {errors.password && (
          <span className="text-red-500 text-sm">
            {errors.password.message}
          </span>
        )}
        <p className="flex items-center gap-2 my-3">
          You don't have account?
          <Link
            to="/register"
            className="flex items-center gap-1 text-orange-400 font-semibold"
          >
            Create one <FaArrowRight />
          </Link>
        </p>

        <button
          type="submit"
          disabled={isSubmitting}
          className="bg-linear-to-r from-[#E86819] via-[#FBB92F] to-[#E86819] w-50 p-3 text-lg font-bold
          [text-shadow:0_0_0.7rem_#813405] rounded-md cursor-pointer mb-5 transition-colors hover:via-[#ff8d6a] duration-300"
        >
          {isSubmitting ? "Logging in..." : "Login"}
        </button>
      </form>
    </div>
  );
};

export default LoginPage;
