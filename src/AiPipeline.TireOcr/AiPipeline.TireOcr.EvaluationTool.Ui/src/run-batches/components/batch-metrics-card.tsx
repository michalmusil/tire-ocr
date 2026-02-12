import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { BatchEvaluation } from "../dtos/get-run-batch-detail-dto";
import {
  CheckCircle2,
  CreditCard,
  ShieldAlert,
  SpellCheck,
  Zap,
  type LucideIcon,
} from "lucide-react";

type BatchMetricsCardProps = {
  metrics: BatchEvaluation["statistics"];
};

export const BatchMetricsCard = ({ metrics }: BatchMetricsCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Metrics Evaluation</CardTitle>
        <CardDescription>
          Automatically calculated metrics relevant for formal evaluation of the
          batch.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Separator />
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5 mt-4">
          <MetricItem
            Icon={CheckCircle2}
            name="Parameter Success Rate"
            value={`${metrics.parameterSuccessRate.toFixed(3)}`}
          />
          <MetricItem
            Icon={ShieldAlert}
            name="False Positive Rate"
            value={`${metrics.falsePositiveRate.toFixed(3)}`}
          />
          <MetricItem
            Icon={SpellCheck}
            name="Average CER"
            value={`${metrics.averageCer.toFixed(3)}`}
          />
          <MetricItem
            Icon={CreditCard}
            name="Average Inference Cost"
            value={`$${metrics.averageInferenceCost.toFixed(5)}`}
          />
          <MetricItem
            Icon={Zap}
            name="Average latency"
            value={`${metrics.averageLatencyMs.toFixed(3)} ms`}
          />
        </div>
      </CardContent>
    </Card>
  );
};

const MetricItem = ({
  Icon,
  name,
  value,
}: {
  Icon: LucideIcon;
  name: string;
  value: string;
}) => {
  return (
    <div className="flex gap-x-3 items-center">
      <Icon size={30} />
      <div className="flex flex-col gap-y-1">
        <span className="text-sm font-medium text-muted-foreground">
          {name}
        </span>
        <span className="text-lg font-bold">{value}</span>
      </div>
    </div>
  );
};
