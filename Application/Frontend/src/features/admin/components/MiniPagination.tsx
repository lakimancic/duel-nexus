
import React from "react";
import type { PaginationProps } from "../types/props.types";

const MiniPagination: React.FC<PaginationProps & { className?: string }> = ({ page, totalPages, onPrev, onNext, className }) => (
  <div className={`flex items-center gap-2 ${className || ""}`}>
    <button
      className="px-3 py-1 rounded-lg border border-violet-400 bg-indigo-950/60 text-violet-200 hover:bg-violet-800 transition-all disabled:opacity-50 text-base font-bold shadow-sm"
      onClick={onPrev}
      disabled={page <= 1}
    >
      &lt;
    </button>
    <span className="text-violet-200 text-base font-semibold px-2 select-none">
      {page} / {totalPages}
    </span>
    <button
      className="px-3 py-1 rounded-lg border border-violet-400 bg-indigo-950/60 text-violet-200 hover:bg-violet-800 transition-all disabled:opacity-50 text-base font-bold shadow-sm"
      onClick={onNext}
      disabled={page >= totalPages}
    >
      &gt;
    </button>
  </div>
);

export default MiniPagination;
