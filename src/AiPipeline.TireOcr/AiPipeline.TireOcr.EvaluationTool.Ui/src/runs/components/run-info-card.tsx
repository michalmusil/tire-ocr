import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { formatDateTime } from "@/core/utils/datetime-utils";
import { getDisplayedDurationFromDatetimeBoundaries } from "@/run-batches/utils/data-utils";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useDeleteRunMutation } from "../mutations/delete-run-mutation";
import { RunsQueryKey } from "../queries/use-runs-query";
import ConfirmationDialog from "@/core/components/confirmation-dialog";
import { Button } from "@/core/components/ui/button";
import { Spinner } from "@/core/components/ui/spinner";
import EditRunDialog from "./edit-run-dialog";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { Separator } from "@/core/components/ui/separator";
import EvaluationResultCategoryBadge from "./evaluation-result-category-badge";

type RunInfoCardProps = {
  evaluationRun: EvaluationRun;
  estimatedCost?: {
    amount: number;
    currency: string;
  } | null;
} & React.HTMLAttributes<HTMLDivElement>;

export const RunInfoCard = ({
  evaluationRun,
  estimatedCost,
  className,
  ...rest
}: RunInfoCardProps) => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const deleteMutation = useDeleteRunMutation();

  const onDeleteConfirmed = () => {
    deleteMutation.mutate(evaluationRun.id, {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: [RunsQueryKey] });
        navigate("/runs");
      },
      onError: (error) => {
        console.log(error);
      },
    });
  };

  return (
    <Card {...rest} className={className}>
      <CardHeader className="flex justify-between">
        <div>
          <CardTitle className="text-2xl break-all min-w-0">
            {evaluationRun.title}
          </CardTitle>
          <CardDescription className="break-all min-w-0">
            {evaluationRun.description}
          </CardDescription>
        </div>
        <div className="flex gap-x-3">
          <EditRunDialog
            evaluationRun={evaluationRun}
            trigger={<Button>Edit</Button>}
          />
          <ConfirmationDialog
            title="Delete"
            description="Are you sure you want to permanently delete this evaluation run?"
            type="destructive"
            confirmText="Delete"
            cancelText="Cancel"
            onConfirm={onDeleteConfirmed}
            trigger={
              <Button disabled={deleteMutation.isPending} variant="destructive">
                {deleteMutation.isPending ? <Spinner /> : "Delete"}
              </Button>
            }
          />
        </div>
      </CardHeader>
      <Separator orientation="horizontal" />
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <InfoItem
            label="Started At"
            value={formatDateTime(evaluationRun.startedAt)}
          />
          <InfoItem
            label="Finished At"
            value={formatDateTime(evaluationRun.finishedAt)}
          />
          <InfoItem
            label="Evaluation Category"
            value={
              <EvaluationResultCategoryBadge
                category={evaluationRun.evaluationResultCategory}
              />
            }
          />
          <InfoItem
            label="Duration"
            value={getDisplayedDurationFromDatetimeBoundaries(
              evaluationRun.startedAt,
              evaluationRun.finishedAt
            )}
          />
          {estimatedCost && (
            <InfoItem
              label="Estimated cost"
              value={`${estimatedCost.amount.toFixed(4)} ${
                estimatedCost.currency
              }`}
            />
          )}
        </div>
      </CardContent>
    </Card>
  );
};

const InfoItem = ({
  label,
  value,
}: {
  label: string;
  value: string | React.ReactNode;
}) => {
  return (
    <div>
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      {typeof value === "string" ? (
        <p className="text-md font-semibold">{value}</p>
      ) : (
        value
      )}
    </div>
  );
};
