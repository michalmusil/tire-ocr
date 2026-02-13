import { z } from "zod";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/forms/form-input";
import { Button } from "@/core/components/ui/button";
import { Spinner } from "@/core/components/ui/spinner";

const formSchema = z.object({
  inferenceStabilityRelativeBatchId: z.string().nullish(),
  annualFixedCostUsd: z.number().nullish(),
  expectedAnnualInferences: z.number().nullish(),
});
export type ExportBatchMetricsFormSchema = z.infer<typeof formSchema>;

type ExportBatchMetricsFormProps = {
  onSubmit: (inputs: ExportBatchMetricsFormSchema) => void;
  error?: string | null;
  exportInProgress: boolean;
};

const ExportBatchMetricsForm = ({
  onSubmit,
  error,
  exportInProgress,
}: ExportBatchMetricsFormProps) => {
  const form = useForm<ExportBatchMetricsFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      inferenceStabilityRelativeBatchId: null,
      annualFixedCostUsd: null,
      expectedAnnualInferences: null,
    },
  });

  return (
    <FormProvider {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className="flex flex-col gap-2 mb-5 items-center">
          <FormInput<ExportBatchMetricsFormSchema>
            label="ID of relative batch to calculate inference stability (IS) against"
            name="inferenceStabilityRelativeBatchId"
            placeholder="Batch GUID"
          />

          <FormInput<ExportBatchMetricsFormSchema>
            label="Annual fixed cost of operation (USD)"
            name="annualFixedCostUsd"
            type="number"
            placeholder="Annual Fixed Cost"
          />

          <FormInput<ExportBatchMetricsFormSchema>
            label="Expected annual inference count"
            name="expectedAnnualInferences"
            type="number"
            placeholder="Inference Count"
          />

          {error && (
            <div className="mt-2 text-sm text-destructive">{error}</div>
          )}

          <Button
            className="mt-4 w-[50%]"
            type="submit"
            variant="default"
            disabled={exportInProgress}
          >
            {exportInProgress && <Spinner />}
            {exportInProgress ? "Exporting..." : "Export"}
          </Button>
        </div>
      </form>
    </FormProvider>
  );
};

export default ExportBatchMetricsForm;
