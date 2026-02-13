import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/core/components/ui/dialog";
import { useExportRawBatchMutation } from "../mutations/export-raw-batch-mutation";
import { useState } from "react";
import { Button } from "@/core/components/ui/button";
import { useExportBatchMetricsMutation } from "../mutations/export-batch-metrics-mutation";
import { Spinner } from "@/core/components/ui/spinner";
import type { ExportBatchMetricsFormSchema } from "./export-batch-metrics-form";
import ExportBatchMetricsForm from "./export-batch-metrics-form";

type ExportBatchDialogProps = {
  batchId: string;
  trigger: React.ReactNode;
};

type DisplayMode = "undecided" | "metrics" | "exporting";

const ExportBatchDialog = ({ batchId, trigger }: ExportBatchDialogProps) => {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [displayMode, setDisplayMode] = useState<DisplayMode>("undecided");
  const exportRawMutation = useExportRawBatchMutation();
  const exportMetricsMutation = useExportBatchMetricsMutation();

  const onExportRaw = () => {
    setDisplayMode("exporting");
    exportRawMutation.mutate(batchId, {
      onSuccess(data) {
        // Save the file using a temporary link
        const url = window.URL.createObjectURL(new Blob([data]));
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute("download", `${batchId}.csv`);
        document.body.appendChild(link);
        link.click();
        link.remove();
        setDisplayMode("undecided");
      },
      onError: (error) => {
        console.log(error);
        setDisplayMode("undecided");
      },
    });
  };

  const onExportMetrics = (inputs: ExportBatchMetricsFormSchema) => {
    setDisplayMode("exporting");
    exportMetricsMutation.mutate(
      {
        batchId,
        inferenceStabilityRelativeBatchId:
          inputs.inferenceStabilityRelativeBatchId,
        annualFixedCostUsd: inputs.annualFixedCostUsd,
        expectedAnnualInferences: inputs.expectedAnnualInferences,
      },
      {
        onSuccess: (data) => {
          // Save the file using a temporary link
          const url = window.URL.createObjectURL(new Blob([data]));
          const link = document.createElement("a");
          link.href = url;
          link.setAttribute("download", `${batchId}-metrics.csv`);
          document.body.appendChild(link);
          link.click();
          link.remove();
          setDisplayMode("undecided");
        },
        onError: (error) => {
          console.log(error);
          setDisplayMode("metrics");
        },
      },
    );
  };

  return (
    <Dialog
      open={isOpen}
      onOpenChange={(isOpen) => {
        setIsOpen(isOpen);
        if (!isOpen) {
          setDisplayMode("undecided");
        }
      }}
    >
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {displayMode === "undecided"
              ? "Choose export type"
              : "Export Metrics"}
          </DialogTitle>
          <DialogDescription>
            {displayMode === "undecided" &&
              "Choose which batch data you want to export"}
            {displayMode === "metrics" &&
              "You can optionally enter additional batch details, which can't be automatically determined, to calculate metrics: EAC (Estimated Annual Cost) and IS (Inference Stability)."}
          </DialogDescription>
        </DialogHeader>
        {displayMode === "exporting" && (
          <div className="flex flex-col items-center gap-2">
            <Spinner className="w-12 h-12" />
          </div>
        )}
        {displayMode === "undecided" && (
          <div className="flex flex-col items-center gap-3">
            <Button className="w-[70%]" onClick={onExportRaw} variant="outline">
              Export Raw Data
            </Button>
            <Button
              className="w-[70%]"
              onClick={() => setDisplayMode("metrics")}
              variant="outline"
            >
              Export Metrics
            </Button>
          </div>
        )}
        {displayMode === "metrics" && (
          <>
            <ExportBatchMetricsForm
              onSubmit={onExportMetrics}
              error={exportMetricsMutation.error?.message}
              exportInProgress={exportMetricsMutation.isPending}
            />
            <DialogFooter>
              <Button
                onClick={() => setDisplayMode("undecided")}
                variant="secondary"
              >
                Back
              </Button>
            </DialogFooter>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default ExportBatchDialog;
