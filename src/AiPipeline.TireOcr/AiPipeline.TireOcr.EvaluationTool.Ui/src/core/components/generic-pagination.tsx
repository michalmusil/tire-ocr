import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationPrevious,
  PaginationNext,
  PaginationEllipsis,
} from "./ui/pagination";

type GenericPaginationProps = {
  currentPage: number;
  totalPages: number;
  getPageHref: (page: number) => string;
} & React.HTMLAttributes<HTMLDivElement>;

export function GenericPagination({
  currentPage,
  totalPages,
  getPageHref,
  ...rest
}: GenericPaginationProps) {
  if (totalPages <= 0) {
    return null;
  }
  const pagesToShow: number[] = [1];

  if (totalPages > 1) {
    const start = Math.max(1, currentPage - 2);
    const end = Math.min(totalPages, currentPage + 2);

    for (let i = start; i <= end; i++) {
      if (i !== 1 && i !== totalPages) {
        pagesToShow.push(i);
      }
    }

    pagesToShow.push(totalPages);
  }

  return (
    <Pagination {...rest}>
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            href={getPageHref(currentPage - 1)}
            aria-disabled={currentPage === 1}
            tabIndex={currentPage === 1 ? -1 : undefined}
            className={
              currentPage === 1 ? "pointer-events-none opacity-50" : undefined
            }
          />
        </PaginationItem>

        {pagesToShow.map((page, index) => (
          <div className="flex flex-row" key={index}>
            {index > 0 && page - pagesToShow[index - 1] > 1 && (
              <PaginationEllipsis />
            )}
            <PaginationItem key={page}>
              <PaginationLink
                href={getPageHref(page)}
                isActive={page === currentPage}
              >
                {page}
              </PaginationLink>
            </PaginationItem>
          </div>
        ))}

        <PaginationItem>
          <PaginationNext
            href={getPageHref(currentPage + 1)}
            aria-disabled={currentPage === totalPages}
            tabIndex={currentPage === totalPages ? -1 : undefined}
            className={
              currentPage === totalPages
                ? "pointer-events-none opacity-50"
                : undefined
            }
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}
