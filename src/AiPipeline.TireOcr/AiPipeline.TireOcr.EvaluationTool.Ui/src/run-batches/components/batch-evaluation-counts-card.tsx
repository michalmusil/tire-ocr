import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import type { BatchEvaluation } from "../dtos/get-run-batch-detail-dto";
import { cn } from "@/core/lib/utils";

type BatchEvaluationCountsCardProps = {
  countsEvaluation: BatchEvaluation["counts"];
};

export const BatchEvaluationCountsCard = ({
  countsEvaluation,
}: BatchEvaluationCountsCardProps) => {
  const totalCorrect =
    countsEvaluation.fullyCorrectCount +
    countsEvaluation.correctMainParametersCount;

  const totalFailed =
    countsEvaluation.failedPreprocessingCount +
    countsEvaluation.failedOcrCount +
    countsEvaluation.failedPostprocessingCount +
    countsEvaluation.failedUnexpectedCount;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Evaluation overview</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-2 md:grid-cols-3 gap-x-4 gap-y-10 items-start">
          <HeadingProperty
            label="Total Count"
            value={`${countsEvaluation.totalCount}`}
          />
          <div className="flex flex-col gap-y-1">
            <HeadingProperty
              label="Correct"
              value={`${totalCorrect}`}
              className="text-green-600 mb-2"
            />
            <PropertyDetails
              details={[
                {
                  label: "fully",
                  value: `${countsEvaluation.fullyCorrectCount}`,
                  colorClass: "text-green-600",
                },
                {
                  label: "main parameters",
                  value: `${countsEvaluation.correctMainParametersCount}`,
                  colorClass: "text-blue-600",
                },
              ]}
            />
          </div>
          <div className="flex flex-col gap-y-1">
            <HeadingProperty
              label="Failed"
              value={`${totalFailed}`}
              className="text-red-700 mb-2"
            />
            <PropertyDetails
              details={[
                {
                  label: "preprocessing",
                  value: `${countsEvaluation.failedPreprocessingCount}`,
                  colorClass: "text-red-600",
                },
                {
                  label: "ocr",
                  value: `${countsEvaluation.failedOcrCount}`,
                  colorClass: "text-red-600",
                },
                {
                  label: "postprocessing",
                  value: `${countsEvaluation.failedPostprocessingCount}`,
                  colorClass: "text-red-600",
                },
                {
                  label: "unexpected",
                  value: `${countsEvaluation.failedUnexpectedCount}`,
                  colorClass: "text-red-600",
                },
              ]}
            />
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

const HeadingProperty = ({
  label,
  value,
  className,
}: { label: string; value: string } & Pick<
  React.HTMLAttributes<HTMLDivElement>,
  "className"
>) => {
  return (
    <div className="space-y-2">
      <p className="text-lg font-semibold text-muted-foreground line-clamp-1">
        {label}
      </p>
      <p className={cn("text-5xl font-bold", className)}>{value}</p>
    </div>
  );
};

const PropertyDetails = ({
  details,
}: {
  details: {
    label: string;
    value: string;
    colorClass: string;
  }[];
}) => {
  return (
    <div className="flex flex-col items-start gap-y-1">
      <div className="w-[70%] h-[2px] bg-muted" />
      {details.map((detail, index) => (
        <TextValuePair
          key={index}
          label={detail.label}
          value={detail.value}
          className={detail.colorClass}
        />
      ))}
    </div>
  );
};

const TextValuePair = ({
  label,
  value,
  className,
}: { label: string; value: string } & Pick<
  React.HTMLAttributes<HTMLDivElement>,
  "className"
>) => {
  return (
    <div className="flex flex-row items-center gap-x-2">
      <p className="text-sm font-medium text-muted-foreground">{`${label}:`}</p>
      <p className={cn("text-md font-semibold", className)}>{value}</p>
    </div>
  );
};
