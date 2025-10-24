import { useMutation } from "@tanstack/react-query";
import type { CreateEvaluationRunFormSchema } from "../components/create-run-form";
import { PostRunResponseSchema } from "../dtos/post-run-response-dto";

const getFormAsFormData = (
  data: CreateEvaluationRunFormSchema,
  image: File
) => {
  const formData = new FormData();
  formData.append("runTitle", data.runTitle);
  formData.append("image", image);
  formData.append("preprocessingType", data.preprocessingType);
  if (data.expectedTireCodeLabel) {
    formData.append("expectedTireCodeLabel", data.expectedTireCodeLabel);
  }
  formData.append("preprocessingType", data.preprocessingType);
  formData.append("ocrType", data.ocrType);
  formData.append("postprocessingType", data.postprocessingType);
  formData.append("dbMatchingType", data.dbMatchingType);
  return formData;
};

const createRun = async (data: CreateEvaluationRunFormSchema) => {
  if (!data.image && !data.imageUrl) {
    throw new Error("Either image or image URL is required");
  }

  const response = data.image
    ? await fetch("/api/v1/Run/WithImage", {
        method: "POST",
        body: getFormAsFormData(data, data.image[0]),
      })
    : await fetch("/api/v1/Run/WithImageUrl", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
  if (!response.ok) {
    throw new Error("Failed to create new evaluation run");
  }
  const json = await response.json();
  const parsed = PostRunResponseSchema.parse(json);
  return parsed.result.id;
};

export const useCreateRunMutation = () =>
  useMutation({ mutationFn: createRun });
