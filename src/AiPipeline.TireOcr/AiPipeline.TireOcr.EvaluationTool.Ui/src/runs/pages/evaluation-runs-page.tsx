import { useSearchParams } from "react-router-dom";
import { EvaluationRunsTable } from "../components/evaluation-runs-table";
import { GenericPagination } from "@/core/components/generic-pagination";
import { useEvaluationRuns } from "../hooks/use-evaluation-runs";
import SpinnerFullpage from "@/core/components/spinner-fullpage";
import ErrorFullpage from "@/core/components/error-fullpage";

const EvaluationRunsPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);

  const { evaluationRuns, pageStatus, getCurrentPagination } =
    useEvaluationRuns(pageNumber, pageSize);

  if (pageStatus.isLoading) return <SpinnerFullpage />;
  if (pageStatus.errorMessage)
    return <ErrorFullpage errorMessage={pageStatus.errorMessage} />;

  return (
    <div className="flex flex-col justify-center items-center">
      <EvaluationRunsTable runs={evaluationRuns?.items ?? []} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={getCurrentPagination().totalPages ?? 1}
        getPageHref={(page) => `/runs?page=${page}&size=${pageSize}`}
      />
    </div>
  );
};

export default EvaluationRunsPage;
