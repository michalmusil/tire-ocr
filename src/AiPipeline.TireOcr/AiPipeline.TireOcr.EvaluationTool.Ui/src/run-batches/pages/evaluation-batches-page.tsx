import { useNavigate, useSearchParams } from "react-router-dom";
import EvaluationRunBatchesTable from "../components/evaluation-run-batches-table";
import GenericPagination from "@/core/components/generic-pagination";
import SpinnerFullpage from "@/core/components/placeholders/spinner-fullpage";
import ErrorFullpage from "@/core/components/placeholders/error-fullpage";
import SearchInput from "@/core/components/search-input";
import { useRunBatchesQuery } from "../queries/use-run-batches-query";

const EvaluationBatchesPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const pageNumber = parseInt(searchParams.get("page") || "1", 10);
  const pageSize = parseInt(searchParams.get("size") || "15", 10);
  const searchTerm = searchParams.get("search");

  const onSearch = (searchTerm: string | null) => {
    let path = `/batches?page=1&size=${pageSize}`;
    if (searchTerm) path += `&search=${searchTerm ?? ""}`;
    navigate(path);
  };

  const { data, isLoading, error } = useRunBatchesQuery(
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
      <EvaluationRunBatchesTable batches={data?.items ?? []} />
      <GenericPagination
        className="mt-5"
        currentPage={pageNumber}
        totalPages={data?.pagination.totalPages ?? 1}
        getPageHref={(page) =>
          `/batches?page=${page}&size=${pageSize}&search=${searchTerm ?? ""}`
        }
      />
    </div>
  );
};

export default EvaluationBatchesPage;
