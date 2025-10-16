import { useSearchParams } from "react-router-dom";
import { EvaluationRunBatchesTable } from "../components/evaluation-run-batches-table";
import { GenericPagination } from "@/core/components/generic-pagination";
import { useRunBatches } from "../hooks/use-run-batches";
import SpinnerFullpage from "@/core/components/spinner-fullpage";
import ErrorFullpage from "@/core/components/error-fullpage";

const EvaluationBatchesPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);

  const { runBatches, pageStatus, getCurrentPagination } = useRunBatches(
    pageNumber,
    pageSize
  );

  if (pageStatus.isLoading) return <SpinnerFullpage />;
  if (pageStatus.errorMessage)
    return <ErrorFullpage errorMessage={pageStatus.errorMessage} />;

  return (
    <div className="flex flex-col justify-center items-center">
      <EvaluationRunBatchesTable batches={runBatches?.items ?? []} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={getCurrentPagination().totalPages ?? 1}
        getPageHref={(page) => `/batches?page=${page}&size=${pageSize}`}
      />
    </div>
  );
};

export default EvaluationBatchesPage;
