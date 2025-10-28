import type { GenericPageStatus } from "@/core/models/generic-page-status";
import { useEffect, useState } from "react";
import {
  PaginatedRunBatchesSchema,
  type PaginatedRunBatches,
} from "../dtos/get-run-batch-dto";
import axios from "axios";

type CurrentPagination = {
  pageNumber: number;
  pageSize: number;
  totalPages: number | null;
  totalCount: number | null;
};

export const useRunBatches = (page: number = 1, pageSize: number = 10) => {
  const [runBatches, setRunBatches] = useState<PaginatedRunBatches | null>(
    null
  );
  const [pageStatus, setPageStatus] = useState<GenericPageStatus>({
    isLoading: false,
    errorMessage: null,
  });

  useEffect(() => {
    loadRunBatches();
  }, [page, pageSize]);

  const getCurrentPagination = (): CurrentPagination => ({
    pageNumber: page,
    pageSize: pageSize,
    totalPages: runBatches?.pagination.totalPages ?? null,
    totalCount: runBatches?.pagination.totalCount ?? null,
  });

  const setIsLoading = (isLoading: boolean) =>
    setPageStatus({
      isLoading: isLoading,
      errorMessage: pageStatus.errorMessage,
    });

  const setError = (errorMessage: string | null) =>
    setPageStatus({
      isLoading: pageStatus.isLoading,
      errorMessage: errorMessage,
    });

  const loadRunBatches = async () => {
    setIsLoading(true);
    try {
      const response = await axios.get(
        `/api/v1/Batch?pageNumber=${page}&pageSize=${pageSize}`
      );
      setIsLoading(false);
      if (response.status !== 200) {
        throw new Error("Failed to fetch evaluation batches");
      }
      const parsed = PaginatedRunBatchesSchema.parse(response.data);
      setRunBatches(parsed);
    } catch (err) {
      setIsLoading(false);
      setError("Failed to fetch evaluation batches");
    }
  };

  return {
    runBatches,
    pageStatus,
    getCurrentPagination,
    setError,
  };
};
