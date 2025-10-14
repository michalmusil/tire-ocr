import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  PaginatedRunBatchesSchema,
  type RunBatch,
} from "../dtos/get-run-batch-dto";
import { EvaluationRunBatchesTable } from "../components/evaluation-run-batches-table";
import { useSearchParams } from "react-router-dom";
import { GenericPagination } from "@/core/components/generic-pagination";

const EvaluationBatchesPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);

  const [batches, setBatches] = useState<RunBatch[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
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
    <div className="flex flex-col justify-center items-center">
      <EvaluationRunBatchesTable batches={batches} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={totalPages}
        getPageHref={(page) => `/batches?page=${page}&size=${pageSize}`}
      />
    </div>
  );
};

export default EvaluationBatchesPage;
