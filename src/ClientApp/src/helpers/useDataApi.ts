import { useRef, useEffect, useState, useReducer } from "react";

// export default function useLoading(callback: () => void) {
//   const savedCallback = useRef<() => void>(callback);
//   const [loading, setLoading] = useState<boolean>(false);
//   const [error, setError] = useState<string | null>(null);
//   const savedLoading = useRef<boolean>(loading);

//   // Remember the latest callback.
//   useEffect(() => {
//     savedCallback.current = callback;
//   }, [callback]);
//   useEffect(() => {
//     savedLoading.current = loading;
//   }, [loading]);

//   // Set up the interval.
//   console.log("loading inside useLoading - before", savedLoading.current);

//   useEffect(() => {
//     setLoading(true);
//     console.log("loading inside useLoading - in useEffect", savedLoading.current);

//     try {
//       savedCallback.current();
//     } catch (error) {
//       setError(error);
//     }

//     setLoading(false);
//   }, []);
//   return [loading, error];
// }

const dataFetchReducer = (state: any, action: any) => {
  switch (action.type) {
    case 'FETCH_INIT':
      return {
        ...state,
        isLoading: true,
        isError: false
      };
    case 'FETCH_SUCCESS':
      return {
        ...state,
        isLoading: false,
        isError: false,
      };
    case 'FETCH_FAILURE':
      return {
        ...state,
        isLoading: false,
        isError: true,
      };
    default:
      throw new Error();
  }
};
export default function useDataApi(callback: () => void) {
  const savedCallback = useRef<() => void>(callback);
  const [state, dispatch] = useReducer(dataFetchReducer, {
    isLoading: false,
    isError: false,
  });

  // useEffect(() => {
  //   savedCallback.current = callback;
  // }, [callback]);
  console.log("loading inside useLoading - before", state.isLoading, savedCallback);
  useEffect(() => {
    let didCancel = false;
    const fetchData = async () => {
      dispatch({ type: 'FETCH_INIT' });
      console.log("loading inside useLoading - after init", state.isLoading);
      try {
        callback();
        if (!didCancel) {
          dispatch({ type: 'FETCH_SUCCESS' });
          console.log("loading inside useLoading - in useEffect", state.isLoading);
        }
      } catch (error) {
        if (!didCancel) {
          dispatch({ type: 'FETCH_FAILURE' });
        }
      }
    };
    fetchData();
    return () => {
      didCancel = true;
    };
  }, [callback]);
  
  return state;
};
