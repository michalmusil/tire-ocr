import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { formatDateTime } from "@/core/utils/datetime-utils";
import { getDisplayedDurationFromDatetimeBoundaries } from "../utils/data-utils";
import ConfirmationDialog from "@/core/components/confirmation-dialog";
import { Button } from "@/core/components/ui/button";
import { useDeleteBatchMutation } from "../mutations/delete-batch-mutation";
import { useNavigate } from "react-router-dom";
import { Spinner } from "@/core/components/ui/spinner";
import { useQueryClient } from "@tanstack/react-query";
import { RunBatchesQueryKey } from "../queries/use-run-batches-query";

type BatchInfoCardProps = {
  title: string;
  batchId: string;
  startedAt?: string | null;
  finishedAt?: string | null;
  totalCost?: {
    amount: number;
    currency: string;
  } | null;
};

export const BatchInfoCard = ({
  title,
  batchId,
  startedAt,
  finishedAt,
  totalCost,
}: BatchInfoCardProps) => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const deleteMutation = useDeleteBatchMutation();

  const onDeleteConfirmed = () => {
    deleteMutation.mutate(batchId, {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: [RunBatchesQueryKey] });
        navigate(-1);
      },
      onError: (error) => {
        console.log(error);
      },
    });
  };

  return (
    <Card>
      <CardHeader className="flex justify-between">
        <div>
          <CardTitle className="text-2xl">{title}</CardTitle>
          <CardDescription>Batch ID: {batchId}</CardDescription>
        </div>
        <ConfirmationDialog
          title="Delete"
          description="Are you sure you want to permanently delete this batch?"
          type="destructive"
          confirmText="Delete"
          cancelText="Cancel"
          onConfirm={onDeleteConfirmed}
          trigger={
            <Button variant="destructive">
              {deleteMutation.isPending ? <Spinner /> : "Delete"}
            </Button>
          }
        />
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <InfoItem label="Started At" value={formatDateTime(startedAt)} />
          <InfoItem label="Finished At" value={formatDateTime(finishedAt)} />
          <InfoItem
            label="Duration"
            value={getDisplayedDurationFromDatetimeBoundaries(
              startedAt,
              finishedAt
            )}
          />
          {totalCost && (
            <InfoItem
              label="Estimated batch cost"
              value={`${totalCost.amount.toFixed(2)} ${totalCost.currency}`}
            />
          )}
        </div>
      </CardContent>
    </Card>
  );
};

const InfoItem = ({ label, value }: { label: string; value: string }) => {
  return (
    <div>
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-md font-semibold">{value}</p>
    </div>
  );
};
