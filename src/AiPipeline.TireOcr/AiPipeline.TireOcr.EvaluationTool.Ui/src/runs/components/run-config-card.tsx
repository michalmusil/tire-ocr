import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { Separator } from "@/core/components/ui/separator";

type RunConfigCardProps = {
  runConfig: EvaluationRun["runConfig"];
};

export const RunConfigCard = ({ runConfig }: RunConfigCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Run Configuration</CardTitle>
        <CardDescription>Pipeline configuration</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-y-2.5">
        <ConfigItem label="Preprocessing" value={runConfig.preprocessingType} />
        <Separator />
        <ConfigItem label="OCR" value={runConfig.ocrType} />
        <Separator />
        <ConfigItem
          label="Postprocessing"
          value={runConfig.postprocessingType}
        />
        <Separator />
        <ConfigItem label="DB Matching" value={runConfig.dbMatchingType} />
      </CardContent>
    </Card>
  );
};

const ConfigItem = ({ label, value }: { label: string; value: string }) => {
  return (
    <div className="flex items-center justify-between">
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-sm font-semibold">{value}</p>
    </div>
  );
};
