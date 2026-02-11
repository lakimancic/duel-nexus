import { useState } from "react";
import type { SearchBarProps } from "../types/props.types";

const SearchBar = ({ onSearch, placeholder }: SearchBarProps) => {
  const [value, setValue] = useState("");

  return (
    <div className="flex flex-1">
      <input
        type="text"
        placeholder={placeholder}
        className="flex-1 border border-violet-500 rounded-l-md py-1 px-2 text-violet-200 outline-none placeholder:text-violet-400/60 focus:border-3"
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />
      <button
        onClick={() => onSearch(value)}
        className="bg-violet-500 text-white px-3 py-2 rounded-r-md cursor-pointer hover:bg-violet-600 transition-colors duration-200"
      >
        Search
      </button>
    </div>
  );
};

export default SearchBar;
