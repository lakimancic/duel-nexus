import { useEffect } from "react";
import type { ModalProps } from "../types/props.types";

const Modal = ({ children, onClose }: ModalProps) => {
  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };

    document.addEventListener("keydown", onKeyDown);
    return () => document.removeEventListener("keydown", onKeyDown);
  }, [onClose]);

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center"
      aria-modal
      role="dialog"
    >
      <div className="absolute inset-0 bg-black/50" onClick={onClose}></div>
      <div className="relative z-10">
        {children}
      </div>
    </div>
  );
};

export default Modal;
