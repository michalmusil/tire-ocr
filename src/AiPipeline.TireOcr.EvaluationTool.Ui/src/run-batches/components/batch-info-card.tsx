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
import EditBatchDialog from "./edit-batch-dialog";
import type { RunBatchDetail } from "../dtos/get-run-batch-detail-dto";
import { Separator } from "@/core/components/ui/separator";
import { useExportRawBatchMutation } from "../mutations/export-raw-batch-mutation";
import ExportBatchDialog from "./export-batch-dialog";

type BatchInfoCardProps = {
  batch: RunBatchDetail;
  totalCost?: {
    amount: number;
    currency: string;
  } | null;
};

export const BatchInfoCard = ({ batch, totalCost }: BatchInfoCardProps) => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const deleteMutation = useDeleteBatchMutation();
  const exportMutation = useExportRawBatchMutation();

  const onDeleteConfirmed = () => {
    deleteMutation.mutate(batch.id, {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: [RunBatchesQueryKey] });
        navigate("/batches");
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
          <CardTitle className="text-2xl">{batch.title}</CardTitle>
          <CardDescription className="whitespace-pre-wrap">
            {batch.description}
          </CardDescription>
        </div>
        <div className="flex gap-3">
          <EditBatchDialog batch={batch} trigger={<Button>Edit</Button>} />
          <ExportBatchDialog
            batchId={batch.id}
            trigger={
              <Button variant="outline" disabled={exportMutation.isPending}>
                {exportMutation.isPending ? <Spinner /> : "Export"}
              </Button>
            }
          />
          <ConfirmationDialog
            title="Delete"
            description="Are you sure you want to permanently delete this batch?"
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
            value={formatDateTime(batch.startedAt)}
          />
          <InfoItem
            label="Finished At"
            value={formatDateTime(batch.finishedAt)}
          />
          <InfoItem
            label="Duration"
            value={getDisplayedDurationFromDatetimeBoundaries(
              batch.startedAt,
              batch.finishedAt,
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
