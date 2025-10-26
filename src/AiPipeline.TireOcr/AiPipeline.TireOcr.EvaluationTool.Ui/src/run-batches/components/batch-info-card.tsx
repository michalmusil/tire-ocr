import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { formatDateTime } from "@/core/utils/datetime-utils";
import { getDisplayedDurationFromDatetimeBoundaries } from "../utils/data-utils";

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
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-2xl">{title}</CardTitle>
        <CardDescription>Batch ID: {batchId}</CardDescription>
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
