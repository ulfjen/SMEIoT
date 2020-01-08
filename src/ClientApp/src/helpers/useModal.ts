import { useState } from "react";

export default function useModal(anchorEl: HTMLElement | null = null): [HTMLElement | null, () => void, (value: HTMLElement) => void] {
  const [modalOpen, setModalOpen] = useState<HTMLElement | null>(anchorEl);

  const handleModalClose = () => {
    setModalOpen(null);
  };

  const openModal = (value: HTMLElement) => {
    setModalOpen(value);
  };

  return [modalOpen, handleModalClose, openModal];
}
