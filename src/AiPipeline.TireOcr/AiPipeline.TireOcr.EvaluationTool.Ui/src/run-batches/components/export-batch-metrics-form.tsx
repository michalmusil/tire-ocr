import { z } from "zod";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/forms/form-input";
import { Button } from "@/core/components/ui/button";
import { Spinner } from "@/core/components/ui/spinner";
import FormCheckbox from "@/core/components/forms/form-checkbox";

const formSchema = z.object({
  inferenceStabilityRelativeBatchIds: z.array(z.string()).nullish(),
  annualFixedCostUsd: z.number().nullish(),
  calculateVariableExpenditure: z.boolean(),
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
      inferenceStabilityRelativeBatchIds: [],
      annualFixedCostUsd: null,
      calculateVariableExpenditure: true,
    },
  });

  return (
    <FormProvider {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className="flex flex-col gap-3 items-center">
          <div className="flex flex-col gap-5 mb-5 items-start">
            <FormInput<ExportBatchMetricsFormSchema>
              label="IDs of related batches (comma-separated) to calculate inference stability (IS) against and average metrics with"
              name="inferenceStabilityRelativeBatchIds"
              placeholder="Batch GUIDs (comma separated)"
              onChange={(e) => {
                const value = e.target.value;
                form.setValue(
                  "inferenceStabilityRelativeBatchIds",
                  value ? value.split(",").map((id) => id.trim()) : [],
                );
              }}
            />

            <FormInput<ExportBatchMetricsFormSchema>
              label="Fixed cost of operation (USD)"
              name="annualFixedCostUsd"
              type="number"
              placeholder="Annual Fixed Cost"
            />

            <FormCheckbox<ExportBatchMetricsFormSchema>
              label="Include variable expenditure in calculation"
              name="calculateVariableExpenditure"
            />
          </div>

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
