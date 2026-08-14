import { useCallback, useEffect, useRef, useState } from "react";

import { getApiErrorMessage } from "../api/httpClient";
import type { AsyncStatus } from "../types/common.types";

export interface UseApiResult<T> {
  data: T | null;
  status: AsyncStatus;
  isLoading: boolean;
  errorMessage: string;
  execute: (request: (signal: AbortSignal) => Promise<T>) => Promise<T | null>;
  reset: () => void;
  setData: (data: T | null) => void;
}

export function useApi<T>(initialData: T | null = null): UseApiResult<T> {
  const [data, setData] = useState<T | null>(initialData);
  const [status, setStatus] = useState<AsyncStatus>("idle");
  const [errorMessage, setErrorMessage] = useState("");
  const requestVersion = useRef(0);
  const activeController = useRef<AbortController | null>(null);

  useEffect(
    () => () => {
      activeController.current?.abort();
    },
    [],
  );

  const execute = useCallback(
    async (request: (signal: AbortSignal) => Promise<T>) => {
      activeController.current?.abort();
      const controller = new AbortController();
      activeController.current = controller;
      const version = ++requestVersion.current;
      setStatus("loading");
      setErrorMessage("");

      try {
        const result = await request(controller.signal);

        if (version === requestVersion.current && !controller.signal.aborted) {
          setData(result);
          setStatus("success");
        }

        return result;
      } catch (error) {
        if (controller.signal.aborted) {
          return null;
        }

        if (version === requestVersion.current) {
          setErrorMessage(getApiErrorMessage(error));
          setStatus("error");
        }

        return null;
      } finally {
        if (activeController.current === controller) {
          activeController.current = null;
        }
      }
    },
    [],
  );

  const reset = useCallback(() => {
    activeController.current?.abort();
    activeController.current = null;
    requestVersion.current += 1;
    setData(initialData);
    setStatus("idle");
    setErrorMessage("");
  }, [initialData]);

  return {
    data,
    status,
    isLoading: status === "loading",
    errorMessage,
    execute,
    reset,
    setData,
  };
}
