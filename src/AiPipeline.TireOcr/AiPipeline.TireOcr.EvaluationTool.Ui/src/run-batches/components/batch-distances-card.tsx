import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { BatchEvaluation } from "../dtos/get-run-batch-detail-dto";

type BatchDistancesCardProps = {
  distances: BatchEvaluation["distances"];
};

export const BatchDistancesCard = ({ distances }: BatchDistancesCardProps) => {
  return (
    <Card>
      <CardHeader className="flex flex-col md:flex-row items-center md:items-start md:justify-between">
        <div>
          <CardTitle>Levenshtein Distance Evaluation</CardTitle>
          <CardDescription>
            Lower values indicate better accuracy
          </CardDescription>
        </div>
        <div className="flex flex-col items-center md:items-end gap-y-2">
          <span className="text-sm font-semibold text-muted-foreground">
            Average char edit distance
          </span>
          <span className="text-4xl font-bold">
            {distances.averageDistance.toFixed(2)}
          </span>
        </div>
      </CardHeader>
      <CardContent>
        <Separator />

        <span className="block text-md font-semibold text-muted-foreground my-4">
          Distance per parameter (Avg.)
        </span>
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
          <DistanceMetric
            label="Vehicle Class"
            value={distances.averageVehicleClassDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Width"
            value={distances.averageWidthDistance.toFixed(2)}
          />

          <DistanceMetric
            label="Diameter"
            value={distances.averageDiameterDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Aspect Ratio"
            value={distances.averageAspectRatioDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Construction"
            value={distances.averageConstructionDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Load Index"
            value={distances.averageLoadIndexDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Load Index 2"
            value={distances.averageLoadIndex2Distance.toFixed(2)}
          />
          <DistanceMetric
            label="Speed Rating"
            value={distances.averageSpeedRatingDistance.toFixed(2)}
          />
          <DistanceMetric
            label="Load Range"
            value={distances.averageLoadRangeDistance.toFixed(2)}
          />
        </div>
      </CardContent>
    </Card>
  );
};

const DistanceMetric = ({ label, value }: { label: string; value: string }) => {
  return (
    <div className="flex flex-col items-start gap-y-1">
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-lg">{value}</p>
    </div>
  );
};
