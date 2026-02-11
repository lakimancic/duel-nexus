import type { PaginationProps } from "../types/props.types";

const PaginationControls = ({
  page,
  totalPages,
  onPrev,
  onNext,
}: PaginationProps) => {
  return (
    <>
      <button
        onClick={onPrev}
        disabled={page === 1}
        className="bg-blue-800/50 px-4 py-2 rounded-lg cursor-pointer text-blue-300 hover:bg-blue-700/60 transition-colors duration-200"
      >
        Prev
      </button>

      <span className="font-bold text-xl mx-5 text-violet-300">
        Page {page} / {totalPages}
      </span>

      <button
        onClick={onNext}
        disabled={page === totalPages}
        className="bg-amber-800/50 px-4 py-2 rounded-lg cursor-pointer text-amber-100 hover:bg-amber-700/60 transition-colors duration-200"
      >
        Next
      </button>
    </>
  );
};

export default PaginationControls;
