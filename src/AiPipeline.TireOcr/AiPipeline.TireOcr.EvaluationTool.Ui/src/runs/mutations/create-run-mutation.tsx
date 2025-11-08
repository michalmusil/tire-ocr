import { useMutation } from "@tanstack/react-query";
import type { CreateEvaluationRunFormSchema } from "../components/create-run-form";
import { PostRunResponseSchema } from "../dtos/post-run-response-dto";
import axios from "axios";

const getFormAsFormData = (
  data: CreateEvaluationRunFormSchema,
  image: File
) => {
  const formData = new FormData();
  formData.append("runTitle", data.runTitle);
  if (data.runDescription)
    formData.append("runDescription", data.runDescription);
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
    ? await axios.post(
        "/api/v1/Run/WithImage",
        getFormAsFormData(data, data.image[0]),
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
    : await axios.post("/api/v1/Run/WithImageUrl", data, {
        headers: {
          "Content-Type": "application/json",
        },
      });
  if (response.status !== 200) {
    throw new Error("Failed to create evaluation run");
  }

  const parsed = PostRunResponseSchema.parse(response.data);
  return parsed.result.id;
};

export const useCreateRunMutation = () =>
  useMutation({ mutationFn: createRun });
