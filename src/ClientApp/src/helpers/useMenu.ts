import { useState } from "react";
import useModal from "./useModal";

export default function useMenu(): [HTMLElement | null, any | null, () => void, (anchorEl: HTMLElement, item: any) => void] {
  const [anchorElement, handleModalClose, openModal] = useModal(null);
  const [menuItem, setMenuItem] = useState<any | null>(null);

  const handleMenuClose = () => {
    handleModalClose();
    setMenuItem(null);
  };

  const openMenu = (anchorEl: HTMLElement, item: any) => {
    // openModal(anchorEl);
    console.log(anchorEl);
    setMenuItem(item);
  };

  return [anchorElement, menuItem, handleMenuClose, openMenu];
}
