export interface PaginationProps {
  page: number;
  totalPages: number;
  onPrev: () => void;
  onNext: () => void;
};

export interface SearchBarProps {
  onSearch: (v: string) => void;
  placeholder: string;
};

export interface ModalProps {
  children: React.ReactNode;
  onClose: () => void;
};
