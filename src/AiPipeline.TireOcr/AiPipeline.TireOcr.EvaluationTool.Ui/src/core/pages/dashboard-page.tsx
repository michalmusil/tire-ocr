import { Link } from "react-router-dom";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Button } from "@/core/components/ui/button";
import { Package, Workflow, ArrowRight } from "lucide-react";

const DashboardPage: React.FC = () => {
  return (
    <div className="container flex flex-col gap-y-8 mx-auto my-8">
      <div className="flex flex-col gap-y-2">
        <h1 className="text-4xl font-bold">Tire OCR Evaluation Tool</h1>
        <p className="text-lg text-muted-foreground max-w-3xl">
          A comprehensive evaluation platform for testing and analyzing tire
          code OCR pipeline performance. Run individual evaluations or batch
          tests to measure accuracy, processing times, and compare different
          pipeline configurations.
        </p>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        <ActionCard
          icon={<Workflow className="w-8 h-8" />}
          title="Evaluation Runs"
          description="View and manage individual OCR evaluation runs. Analyze detailed results including accuracy metrics, processing times, and tire code comparisons."
          actions={[
            { label: "View Runs", to: "/runs" },
            { label: "Create Run", to: "/create-run" },
          ]}
        />

        <ActionCard
          icon={<Package className="w-8 h-8" />}
          title="Evaluation Batches"
          description="Execute and review batch evaluations across multiple images. Compare aggregate statistics and identify performance patterns."
          actions={[
            { label: "View Batches", to: "/batches" },
            { label: "Create Batch", to: "/create-batch" },
          ]}
        />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Key Features</CardTitle>
          <CardDescription>
            What you can do with this evaluation tool
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <FeatureItem
              title="Pipeline Testing"
              description="Test different preprocessing, OCR, and postprocessing configurations"
            />
            <FeatureItem
              title="Accuracy Metrics"
              description="Measure character edit distance and parameter-level accuracy"
            />
            <FeatureItem
              title="Performance Analysis"
              description="Track execution times for each pipeline step"
            />
            <FeatureItem
              title="Cost Estimation"
              description="Monitor estimated costs for API-based OCR services"
            />
            <FeatureItem
              title="Batch Processing"
              description="Run evaluations on multiple images simultaneously"
            />
            <FeatureItem
              title="Detailed Comparisons"
              description="Compare detected codes with expected results parameter by parameter"
            />
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

const ActionCard = ({
  icon,
  title,
  description,
  actions,
}: {
  icon: React.ReactNode;
  title: string;
  description: string;
  actions: Array<{
    label: string;
    to: string;
  }>;
}) => {
  return (
    <Card className="flex-1">
      <CardHeader>
        <div className="flex items-center gap-3">
          <div className="text-primary">{icon}</div>
          <CardTitle>{title}</CardTitle>
        </div>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex gap-3">
          {actions.map((action, index) => (
            <Link key={index} to={action.to}>
              <Button
                className="flex gap-x-2"
                variant={index === 0 ? "default" : "outline"}
              >
                {action.label}
                <ArrowRight className="w-4 h-4" />
              </Button>
            </Link>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};

const FeatureItem = ({
  title,
  description,
}: {
  title: string;
  description: string;
}) => {
  return (
    <div className="space-y-2">
      <h3 className="font-semibold">{title}</h3>
      <p className="text-sm text-muted-foreground">{description}</p>
    </div>
  );
};

export default DashboardPage;
