import { useAuth } from "../hooks/useAuth";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { FaArrowRight } from "react-icons/fa";
import Logo from "../../../assets/images/logo.png";

const registerSchema = z.object({
  username: z.string().min(3, "Username must be at least 3 characters"),
  email: z.string().min(1, "Email is required").email("Invalid email"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

type RegisterForm = z.infer<typeof registerSchema>;

const RegisterPage = () => {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterForm>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterForm) => {
    try {
      await registerUser(data);
      navigate("/lobby");
    } catch (err) {
      console.error(err);
      alert("Registration failed");
    }
  };

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <div className="h-full w-full backdrop-blur-xl absolute"></div>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="relative flex flex-col gap-2 w-120 bg-[#091b33]/80 text-white p-8 items-center pt-30 rounded-xl"
      >
        <img src={Logo} alt="logo" className="absolute -top-30 w-[90%]" />
        <h1 className="text-2xl font-bold mb-5 text-cyan-200 [text-shadow:0_0_0.8rem_#297ee9]">
          Create an account
        </h1>

        <input
          {...register("username")}
          placeholder="Username"
          className="p-2 w-full rounded border border-blue-300 placeholder:text-cyan-200/50 text-blue-300 outline-none"
        />
        {errors.username && (
          <span className="text-red-500 text-sm">
            {errors.username.message}
          </span>
        )}

        <input
          {...register("email")}
          placeholder="Email"
          className="p-2 mt-5 w-full rounded border border-blue-300 placeholder:text-cyan-200/50 text-blue-300 outline-none"
        />
        {errors.email && (
          <span className="text-red-500 text-sm">{errors.email.message}</span>
        )}

        <input
          {...register("password")}
          type="password"
          placeholder="Password"
          className="p-2 mt-5 w-full rounded border border-blue-300 placeholder:text-cyan-200/50 text-blue-300 outline-none"
        />
        {errors.password && (
          <span className="text-red-500 text-sm">
            {errors.password.message}
          </span>
        )}

        <p className="flex items-center gap-2 my-3">
          Already have an account?
          <Link
            to="/login"
            className="flex items-center gap-1 text-blue-400 font-semibold"
          >
            Log in <FaArrowRight />
          </Link>
        </p>

        <button
          type="submit"
          disabled={isSubmitting}
          className="bg-linear-to-r from-[#2a83ea] via-[#5fdefb] to-[#2a83ea] w-50 p-3 text-lg font-bold
          [text-shadow:0_0_0.7rem_#813405] rounded-md cursor-pointer mb-5 transition-colors hover:via-[#8789fd] duration-300"
        >
          {isSubmitting ? "Registering..." : "Register"}
        </button>
      </form>
    </div>
  );
};

export default RegisterPage;
