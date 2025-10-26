import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { formatDateTime } from "@/core/utils/datetime-utils";
import { getDisplayedDurationFromDatetimeBoundaries } from "@/run-batches/utils/data-utils";

type RunInfoCardProps = {
  title: string;
  runId: string;
  startedAt?: string | null;
  finishedAt?: string | null;
  estimatedCost?: {
    amount: number;
    currency: string;
  } | null;
};

export const RunInfoCard = ({
  title,
  runId,
  startedAt,
  finishedAt,
  estimatedCost,
}: RunInfoCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-2xl">{title}</CardTitle>
        <CardDescription>Run ID: {runId}</CardDescription>
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

const InfoItem = ({ label, value }: { label: string; value: string }) => {
  return (
    <div>
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-md font-semibold">{value}</p>
    </div>
  );
};
