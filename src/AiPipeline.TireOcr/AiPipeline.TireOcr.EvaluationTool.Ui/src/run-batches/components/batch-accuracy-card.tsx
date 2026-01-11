import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { BatchEvaluation } from "../dtos/get-run-batch-detail-dto";

type BatchAccuracyCardProps = {
  averageCer: BatchEvaluation["averageCer"];
  distances: BatchEvaluation["distances"];
};

export const BatchAccuracyCard = ({
  averageCer,
  distances,
}: BatchAccuracyCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Accuracy Evaluation</CardTitle>
        <CardDescription>Lower values indicate better accuracy</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          <div className="flex space-x-10">
            <div className="space-y-2">
              <p className="text-lg font-semibold text-muted-foreground">
                Char edit distance (Avg.)
              </p>
              <p className="text-5xl font-bold">
                {distances.averageDistance.toFixed(2)}
              </p>
            </div>
            <div className="space-y-2">
              <p className="text-lg font-semibold text-muted-foreground">
                CER (Avg.)
              </p>
              <p className="text-5xl font-bold">{averageCer.toFixed(2)}</p>
            </div>
          </div>

          <Separator />

          <span className="text-md font-semibold text-muted-foreground">
            Distance per parameter (Avg.)
          </span>
          <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mt-4">
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
