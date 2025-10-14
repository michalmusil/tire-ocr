import React, { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import axios from "axios";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { PaginatedEvaluationRunsSchema } from "../dtos/get-evaluation-run-dto";
import { EvaluationRunsTable } from "../components/evaluation-runs-table";
import { GenericPagination } from "@/core/components/generic-pagination";

const EvaluationRunsPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);

  const [runs, setRuns] = useState<EvaluationRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const fetchRuns = async () => {
      setLoading(true);
      try {
        const response = await axios.get(
          `/api/v1/Run?pageNumber=${pageNumber}&pageSize=${pageSize}`
        );
        const parsedData = PaginatedEvaluationRunsSchema.parse(response.data);
        setRuns(parsedData.items);
        setTotalPages(parsedData.pagination.totalPages);
      } catch (err) {
        setError("Failed to fetch or validate evaluation runs");
      }
      setLoading(false);
    };

    fetchRuns();
  }, [pageNumber, pageSize]);

  if (loading) return <div>Loading...</div>;
  if (error)
    return (
      <div className="text-red-500 text-center flex flex-row justify-center">
        {error}
      </div>
    );

  return (
    <div className="flex flex-col justify-center items-center">
      <EvaluationRunsTable runs={runs} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={totalPages}
        getPageHref={(page) => `/runs?page=${page}&size=${pageSize}`}
      />
    </div>
  );
};

export default EvaluationRunsPage;
