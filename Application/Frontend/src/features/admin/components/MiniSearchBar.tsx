
import React, { useState } from "react";
import type { SearchBarProps } from "../types/props.types";

const MiniSearchBar: React.FC<SearchBarProps & { className?: string }> = ({ onSearch, placeholder, className }) => {
  const [value, setValue] = useState("");
  return (
    <div className={`flex items-center gap-2 ${className || ""}`}>
      <input
        type="text"
        value={value}
        onChange={e => setValue(e.target.value)}
        placeholder={placeholder || "Search..."}
        className="p-2 rounded-lg border border-violet-400 bg-indigo-950/60 text-violet-200 placeholder:text-violet-300/60 outline-none focus:ring-2 focus:ring-violet-500 transition-all text-base min-w-0"
        style={{ minWidth: 0 }}
      />
      <button
        type="button"
        onClick={() => onSearch(value)}
        className="px-4 py-2 rounded-lg bg-violet-700 text-white font-semibold shadow-md hover:bg-violet-800 transition-all"
      >
        Search
      </button>
    </div>
  );
};

export default MiniSearchBar;
