import { useState, useCallback } from "react";

// the callback needs to be memorized because setState causes a rerender.
// and we need the value to be updated but not the callback
// for example, openMenu uses openModal and sets a value before doing so,
// we will need the openModal remains the same to be able to set a value.
// otherwise the rerender would erase the previous openModal function.
export default function useModal<T>(): [boolean, (value: T) => void, () => void, T | undefined] {
  const [open, setOpen] = useState<boolean>(false);
  const [value, setValue] = useState<T | undefined>();

  const closeModal = useCallback(() => {
    setOpen(false);
    setValue(undefined);
  }, []);

  const openModal = useCallback((value: T) => {
    setOpen(true);
    setValue(value);
  }, []);

  return [open, openModal, closeModal, value];
}
