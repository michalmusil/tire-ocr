import { useSearchParams } from "react-router-dom";
import { EvaluationRunsTable } from "../components/evaluation-runs-table";
import GenericPagination from "@/core/components/generic-pagination";
import SpinnerFullpage from "@/core/components/placeholders/spinner-fullpage";
import ErrorFullpage from "@/core/components/placeholders/error-fullpage";
import { useNavigate } from "react-router-dom";
import { useRunsQuery } from "../queries/use-runs-query";
import SearchInput from "@/core/components/search-input";

const EvaluationRunsPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);
  const searchTerm = searchParams.get("search");

  const onSearch = (searchTerm: string | null) => {
    let path = `/runs?page=1&size=${pageSize}`;
    if (searchTerm) path += `&search=${searchTerm ?? ""}`;
    navigate(path);
  };

  const { data, isLoading, error } = useRunsQuery(
    pageNumber,
    pageSize,
    searchTerm
  );

  if (isLoading) return <SpinnerFullpage />;
  if (error) return <ErrorFullpage errorMessage={error.message} />;

  return (
    <div className="flex flex-col justify-center items-center">
      <div className="w-full">
        <div className="flex justify-start w-sm md:w-md lg:w-lg mb-8">
          <SearchInput onSearchChanged={onSearch} initialValue={searchTerm} />
        </div>
      </div>
      <EvaluationRunsTable runs={data?.items ?? []} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={data?.pagination.totalPages ?? 1}
        getPageHref={(page) =>
          `/runs?page=${page}&size=${pageSize}&search=${searchTerm ?? ""}`
        }
      />
    </div>
  );
};

export default EvaluationRunsPage;
