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
          <TableHead>Id</TableHead>
          <TableHead>Title</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {batches.map((batch) => (
          <TableRow
            key={batch.id}
            onClick={() => handleRowClick(batch.id)}
            className="cursor-pointer hover:bg-muted-foreground"
          >
            <TableCell>{batch.id}</TableCell>
            <TableCell>{batch.title}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};
