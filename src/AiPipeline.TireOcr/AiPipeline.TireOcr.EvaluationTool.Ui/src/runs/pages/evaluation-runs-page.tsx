import React, { useState, useEffect } from "react";
import axios from "axios";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { PaginatedEvaluationRunsSchema } from "../dtos/get-evaluation-run-dto";
import { EvaluationRunsTable } from "../components/evaluation-runs-table";

const EvaluationRunsPage: React.FC = () => {
  const [runs, setRuns] = useState<EvaluationRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(15);
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
    <div className="flex justify-center items-center">
      <EvaluationRunsTable runs={runs} />
    </div>
  );
};

export default EvaluationRunsPage;
