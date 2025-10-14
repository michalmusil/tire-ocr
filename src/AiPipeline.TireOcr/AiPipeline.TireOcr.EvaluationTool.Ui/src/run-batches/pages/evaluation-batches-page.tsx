import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  PaginatedRunBatchesSchema,
  type RunBatch,
} from "../dtos/get-run-batch-dto";
import { EvaluationRunBatchesTable } from "../components/evaluation-run-batches-table";

const EvaluationBatchesPage: React.FC = () => {
  const [batches, setBatches] = useState<RunBatch[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const fetchBatches = async () => {
      setLoading(true);
      try {
        const response = await axios.get(
          `/api/v1/Batch?pageNumber=${pageNumber}&pageSize=${pageSize}`
        );
        const parsedData = PaginatedRunBatchesSchema.parse(response.data);
        setBatches(parsedData.items);
        setTotalPages(parsedData.pagination.totalPages);
      } catch (err) {
        setError("Failed to fetch or validate evaluation batches");
      }
      setLoading(false);
    };

    fetchBatches();
  }, [pageNumber, pageSize]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;

  return (
    <div className="flex justify-center items-center">
      <EvaluationRunBatchesTable batches={batches} />
    </div>
  );
};

export default EvaluationBatchesPage;
