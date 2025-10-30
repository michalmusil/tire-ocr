import { useNavigate } from "react-router-dom";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/core/components/ui/table";
import type { RunBatch } from "../dtos/get-run-batch-dto";
import { getDisplayedDurationFromDatetimeBoundaries } from "../utils/data-utils";

type EvaluationRunBatchesTableProps = {
  batches: RunBatch[];
};

const EvaluationRunBatchesTable = ({
  batches,
}: EvaluationRunBatchesTableProps) => {
  const navigate = useNavigate();

  const handleRowClick = (batchId: string) => {
    navigate(`/batches/${batchId}`);
  };

  return (
    <Table>
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
              {getDisplayedDurationFromDatetimeBoundaries(
                batch.startedAt,
                batch.finishedAt
              )}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default EvaluationRunBatchesTable;
