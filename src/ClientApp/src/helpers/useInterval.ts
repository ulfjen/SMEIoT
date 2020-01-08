import { useRef, useEffect } from "react";

export default function useInterval(callback: () => void, delay: number) {
  const savedCallback = useRef<() => void>(callback);

  // Remember the latest callback.
  useEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  // Set up the interval.
  useEffect(() => {
    function tick() {
      // if (savedCallback.current) {
        savedCallback.current();
      // }
    }
    if (delay !== null) {
      let id = setInterval(tick, delay);
      return () => clearInterval(id);
    }
  }, [delay]);
}
