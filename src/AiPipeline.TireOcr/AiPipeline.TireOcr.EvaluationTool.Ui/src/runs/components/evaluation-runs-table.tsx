import { useNavigate } from "react-router-dom";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/core/components/ui/table";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { Check, X } from "lucide-react";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/core/components/ui/tooltip";
import EvaluationResultCategoryBadge from "./evaluation-result-category-badge";

type EvaluationRunsTableProps = {
  runs: EvaluationRun[];
  displayCaption?: boolean;
};

export const EvaluationRunsTable = ({ runs }: EvaluationRunsTableProps) => {
  const navigate = useNavigate();

  const handleRowClick = (runId: string) => {
    navigate(`/runs/${runId}`);
  };

  const handleRowAuxClick = (runId: string) => {
    window.open(`/runs/${runId}`);
  };

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Title</TableHead>
          <TableHead className="flex justify-center items-center">
            Run completed
          </TableHead>
          <TableHead>Duration</TableHead>
          <TableHead>Expected code</TableHead>
          <TableHead>Result/Failure Reason</TableHead>
          <TableHead>Evaluation Category</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {runs.map((run) => {
          return (
            <TableRow
              key={run.id}
              onClick={() => handleRowClick(run.id)}
              onAuxClick={() => handleRowAuxClick(run.id)}
              className="cursor-pointer"
            >
              <TableCell>{run.title}</TableCell>
              <TableCell className="flex justify-center">
                {run.failure ? (
                  <X className="text-red-500" />
                ) : (
                  <Check className="text-green-500" />
                )}
              </TableCell>
              <TableCell>
                {(run.totalExecutionDurationMs / 1000).toFixed(2)}s
              </TableCell>
              <TableCell>
                {run.evaluation?.expectedTireCode?.processedCode ?? "-"}
              </TableCell>
              <TableCell>
                {run.failure ? (
                  run.failure.failureReason
                ) : (
                  <Tooltip>
                    <TooltipTrigger>
                      {run.postprocessingResult?.postprocessedTireCode
                        .processedCode ?? "Unknown"}
                    </TooltipTrigger>
                    <TooltipContent>
                      {`Raw: ${
                        run.postprocessingResult?.postprocessedTireCode
                          .rawCode ?? "Unknown"
                      }`}
                    </TooltipContent>
                  </Tooltip>
                )}
              </TableCell>
              <TableCell>
                <EvaluationResultCategoryBadge
                  category={run.evaluationResultCategory}
                />
              </TableCell>
            </TableRow>
          );
        })}
      </TableBody>
    </Table>
  );
};
