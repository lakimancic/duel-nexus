import React from "react";

type SpinnerProps = React.HTMLAttributes<HTMLSpanElement> & {
  size?: string;
  color?: string;
  ariaLabel?: string;
};

const Spinner: React.FC<SpinnerProps> = ({
  size = "w-8 h-8",
  color = "text-gray-500",
  ariaLabel = "Loading...",
  className = "",
  ...rest
}) => {
  return (
    <span
      role="status"
      aria-label={ariaLabel}
      className={`inline-block animate-spin ${size} ${color} ${className}`.trim()}
      {...rest}
    >
      <svg
        className="w-full h-full"
        viewBox="0 0 24 24"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
      >
        <circle
          className="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="4"
          fill="none"
        />
        <path
          className="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        />
      </svg>
    </span>
  );
};

export default Spinner;
