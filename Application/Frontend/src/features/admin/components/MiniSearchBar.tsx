import React from "react";

interface MiniSearchBarProps {
  value: string;
  onChange: (v: string) => void;
  placeholder?: string;
  className?: string;
}

const MiniSearchBar: React.FC<MiniSearchBarProps> = ({ value, onChange, placeholder, className }) => (
  <input
    type="text"
    value={value}
    onChange={e => onChange(e.target.value)}
    placeholder={placeholder || "Search..."}
    className={`px-2 py-1 rounded border border-violet-400 bg-indigo-950/60 text-violet-200 placeholder:text-violet-300/60 outline-none focus:ring-2 focus:ring-violet-500 transition-all text-sm ${className || ""}`}
    style={{ minWidth: 0 }}
  />
);

export default MiniSearchBar;
