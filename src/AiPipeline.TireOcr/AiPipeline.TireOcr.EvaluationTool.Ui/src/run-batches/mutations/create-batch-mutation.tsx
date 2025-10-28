import { useMutation } from "@tanstack/react-query";
import type { CreateBatchFormSchema } from "../components/create-run-batch-form";
import { PostRunBatchResponseSchema } from "../dtos/post-run-batch-response-dto";
import axios from "axios";

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

  const response = await axios.post("/api/v1/Batch/Form", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
  if (response.status !== 200) {
    throw new Error("Failed to create evaluation batch");
  }
  const parsed = PostRunBatchResponseSchema.parse(response.data);
  return parsed.result.id;
};

export const useCreateBatchMutation = () =>
  useMutation({ mutationFn: createBatch });
