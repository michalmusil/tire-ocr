import type { GenericPageStatus } from "@/core/models/generic-page-status";
import { useEffect, useState } from "react";
import {
  PaginatedEvaluationRunsSchema,
  type PaginatedEvaluationRuns,
} from "../dtos/get-evaluation-run-dto";
import axios from "axios";

type CurrentPagination = {
  pageNumber: number;
  pageSize: number;
  totalPages: number | null;
  totalCount: number | null;
};

export const useEvaluationRuns = (page: number = 1, pageSize: number = 10) => {
  const [evaluationRuns, setEvaluationRuns] =
    useState<PaginatedEvaluationRuns | null>(null);
  const [pageStatus, setPageStatus] = useState<GenericPageStatus>({
    isLoading: false,
    errorMessage: null,
  });

  useEffect(() => {
    loadEvaluationRuns();
  }, [page, pageSize]);

  const getCurrentPagination = (): CurrentPagination => ({
    pageNumber: page,
    pageSize: pageSize,
    totalPages: evaluationRuns?.pagination.totalPages ?? null,
    totalCount: evaluationRuns?.pagination.totalCount ?? null,
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

  const loadEvaluationRuns = async () => {
    setIsLoading(true);
    try {
      const response = await axios.get(
        `/api/v1/Run?pageNumber=${page}&pageSize=${pageSize}`
      );
      setIsLoading(false);
      if (response.status !== 200) {
        throw new Error("Failed to fetch evaluation runs");
      }
      const parsed = PaginatedEvaluationRunsSchema.parse(response.data);
      setEvaluationRuns(parsed);
    } catch (err) {
      setIsLoading(false);
      setError("Failed to fetch evaluation runs");
    }
  };

  return {
    evaluationRuns,
    pageStatus,
    getCurrentPagination,
    setError,
  };
};
