import { useState, useCallback } from "react";
import useModal from "./useModal";

export default function useMenu<T>(): [boolean, HTMLElement | null, (el: HTMLElement, value: T) => void, () => void, T | undefined] {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [open, openModal, closeModal, value] = useModal<T>();

  const closeMenu = useCallback(() => {
    closeModal();
    setAnchorEl(null);
  }, [closeModal]);

  const openMenu = useCallback((el: HTMLElement, value: T) => {
    setAnchorEl(el);
    openModal(value);
  }, [openModal]);

  return [open, anchorEl, openMenu, closeMenu, value];
}
