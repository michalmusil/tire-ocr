import { useNavigate } from "react-router-dom";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/core/components/ui/table";
import type { RunBatch } from "../dtos/get-run-batch-dto";

type EvaluationRunBatchesTableProps = {
  batches: RunBatch[];
};

const getDisplayedDuration = (
  startedAtStr?: string | null,
  endedAtStr?: string | null
) => {
  const startedAt = startedAtStr ? new Date(startedAtStr) : null;
  const endedAt = endedAtStr ? new Date(endedAtStr) : null;
  const duration =
    startedAt && endedAt ? endedAt.getTime() - startedAt.getTime() : null;

  const durationInSeconds = duration ? duration / 1000 : null;
  const displayedDuration = durationInSeconds
    ? durationInSeconds > 60
      ? `${(durationInSeconds / 60).toFixed(0)}min ${(
          durationInSeconds % 60
        ).toFixed(0)}s`
      : `${durationInSeconds.toFixed(2)}s`
    : "-";

  return displayedDuration;
};

export const EvaluationRunBatchesTable = ({
  batches,
}: EvaluationRunBatchesTableProps) => {
  const navigate = useNavigate();

  const handleRowClick = (batchId: string) => {
    navigate(`/batches/${batchId}`);
  };

  return (
    <Table>
      <TableCaption>Most recent evaluation run batches</TableCaption>
      <TableHeader>
        <TableRow>
          <TableHead>Title</TableHead>
          <TableHead>Number of evaluations</TableHead>
          <TableHead>Created at</TableHead>
          <TableHead>Duration</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {batches.map((batch) => (
          <TableRow
            key={batch.id}
            onClick={() => handleRowClick(batch.id)}
            className="cursor-pointer hover:bg-muted-foreground"
          >
            <TableCell>{batch.title}</TableCell>
            <TableCell>{batch.numberOfEvaluations}</TableCell>
            <TableCell>{batch.createdAt}</TableCell>
            <TableCell>
              {getDisplayedDuration(batch.startedAt, batch.finishedAt)}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};
