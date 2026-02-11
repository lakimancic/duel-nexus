import React from "react";

interface MiniPaginationProps {
  page: number;
  pageCount: number;
  onPageChange: (page: number) => void;
  className?: string;
}

const MiniPagination: React.FC<MiniPaginationProps> = ({ page, pageCount, onPageChange, className }) => (
  <div className={`flex items-center gap-2 ${className || ""}`}>
    <button
      className="px-2 py-1 rounded bg-violet-700 text-white disabled:opacity-50"
      onClick={() => onPageChange(page - 1)}
      disabled={page <= 1}
    >
      &lt;
    </button>
    <span className="text-violet-200 text-xs">
      {page} / {pageCount}
    </span>
    <button
      className="px-2 py-1 rounded bg-violet-700 text-white disabled:opacity-50"
      onClick={() => onPageChange(page + 1)}
      disabled={page >= pageCount}
    >
      &gt;
    </button>
  </div>
);

export default MiniPagination;
