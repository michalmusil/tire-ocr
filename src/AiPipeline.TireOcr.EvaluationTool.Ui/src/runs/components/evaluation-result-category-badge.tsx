import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/core/components/ui/tooltip";
import type { EvaluationResultCategory } from "@/core/models/evaluation-result-category";

type EvaluationResultCategoryBadgeProps = {
  category: EvaluationResultCategory;
};

const EvaluationResultCategoryBadge = ({
  category,
}: EvaluationResultCategoryBadgeProps) => {
  return (
    <Tooltip>
      <TooltipTrigger>
        <div className="">
          <span className={`${getCategoryColor(category)} text-md font-bold`}>
            {getCategoryShortcut(category)}
          </span>
        </div>
      </TooltipTrigger>
      <TooltipContent>{category}</TooltipContent>
    </Tooltip>
  );
};

const getCategoryColor = (category: EvaluationResultCategory) => {
  switch (category) {
    case "FullyCorrect":
      return "text-green-600";
    case "CorrectInMainParameters":
      return "text-blue-600";
    case "NoCodeDetectedPreprocessing":
    case "NoCodeDetectedOcr":
    case "NoCodeDetectedPostprocessing":
    case "NoCodeDetectedUnexpected":
      return "text-orange-600";
    case "FalsePositive":
      return "text-red-600";
    case "InsufficientExtraction":
      return "text-red-400";
    default:
      return "text-gray-600";
  }
};

const getCategoryShortcut = (category: EvaluationResultCategory) => {
  switch (category) {
    case "FullyCorrect":
      return "FC";
    case "CorrectInMainParameters":
      return "CMP";
    case "NoCodeDetectedPreprocessing":
      return "NCDP";
    case "NoCodeDetectedOcr":
      return "NDCO";
    case "NoCodeDetectedPostprocessing":
      return "NCDO";
    case "NoCodeDetectedUnexpected":
      return "NCDU";
    case "FalsePositive":
      return "FP";
    case "InsufficientExtraction":
      return "IE";
    default:
      return "N/A";
  }
};

export default EvaluationResultCategoryBadge;
