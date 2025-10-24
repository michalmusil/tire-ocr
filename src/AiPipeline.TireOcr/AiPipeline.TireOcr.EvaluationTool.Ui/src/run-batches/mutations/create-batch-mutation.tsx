import { useMutation } from "@tanstack/react-query";
import type { CreateBatchFormSchema } from "../components/create-run-batch-form";
import { PostRunBatchResponseSchema } from "../dtos/post-run-batch-response-dto";

const createBatch = async (data: CreateBatchFormSchema) => {
  const formData = new FormData();
  if (data.runTitle) formData.append("runTitle", data.runTitle);
  formData.append(
    "imageUrlsWithExpectedTireCodeLabelsCsv",
    data.imageUrlsWithExpectedTireCodeLabelsCsv[0]
  );
  formData.append("processingBatchSize", data.processingBatchSize.toString());
  formData.append("preprocessingType", data.preprocessingType);
  formData.append("ocrType", data.ocrType);
  formData.append("postprocessingType", data.postprocessingType);
  formData.append("dbMatchingType", data.dbMatchingType);

  const response = await fetch("/api/v1/Batch/Form", {
    method: "POST",
    body: formData,
  });
  if (!response.ok) {
    throw new Error("Failed to create new evaluation run batch");
  }
  const json = await response.json();
  const parsed = PostRunBatchResponseSchema.parse(json);
  return parsed.result.id;
};

export const useCreateBatchMutation = () =>
  useMutation({ mutationFn: createBatch });
