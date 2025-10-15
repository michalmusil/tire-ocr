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
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";

type EvaluationRunsTableProps = {
  runs: EvaluationRun[];
};

export const EvaluationRunsTable = ({ runs }: EvaluationRunsTableProps) => {
  const navigate = useNavigate();

  const handleRowClick = (runId: string) => {
    navigate(`/runs/${runId}`);
  };

  return (
    <Table>
      <TableCaption>Most recent evaluation runs</TableCaption>
      <TableHeader>
        <TableRow>
          <TableHead>Title</TableHead>
          <TableHead>Status</TableHead>
          <TableHead>Duration</TableHead>
          <TableHead>Result/Failure Reason</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {runs.map((run) => (
          <TableRow
            key={run.id}
            onClick={() => handleRowClick(run.id)}
            className="cursor-pointer"
          >
            <TableCell>{run.title}</TableCell>
            <TableCell>{run.failure ? "🔴 Failed" : "🟢 Success"}</TableCell>
            <TableCell>
              {(run.totalExecutionDurationMs / 1000).toFixed(2)}s
            </TableCell>
            <TableCell>
              {run.failure
                ? run.failure.failureReason
                : run.postprocessingResult?.postprocessedTireCode
                    .processedCode ?? "Unknown"}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};
