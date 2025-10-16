import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { formatDateTime } from "@/core/utils/datetime-utils";
import { getDisplayedBatchDuration } from "../utils/data-utils";

type BatchInfoCardProps = {
  title: string;
  batchId: string;
  startedAt?: string | null;
  finishedAt?: string | null;
};

export const BatchInfoCard = ({
  title,
  batchId,
  startedAt,
  finishedAt,
}: BatchInfoCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-2xl">{title}</CardTitle>
        <CardDescription>Batch ID: {batchId}</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <div>
            <p className="text-sm font-medium text-muted-foreground">
              Started At
            </p>
            <p className="text-sm">{formatDateTime(startedAt)}</p>
          </div>
          <div>
            <p className="text-sm font-medium text-muted-foreground">
              Finished At
            </p>
            <p className="text-sm">{formatDateTime(finishedAt)}</p>
          </div>
          <div>
            <p className="text-sm font-medium text-muted-foreground">
              Duration
            </p>
            <p className="text-sm">
              {getDisplayedBatchDuration(startedAt, finishedAt)}
            </p>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};
