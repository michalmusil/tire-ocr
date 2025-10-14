import React, { useState } from "react";
import axios from "axios";
import {
  CreateEvaluationRunForm,
  type CreateEvaluationRunFormSchema,
} from "../components/create-run-form";

const CreateRunPage: React.FC = () => {
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onSubmit = async (data: CreateEvaluationRunFormSchema) => {
    setSubmitting(true);
    setError(null);
    setSuccess(null);

    const formData = new FormData();
    formData.append("runTitle", data.runTitle);
    if (data.expectedTireCodeLabel) {
      formData.append("expectedTireCodeLabel", data.expectedTireCodeLabel);
    }
    formData.append("preprocessingType", data.preprocessingType);
    formData.append("ocrType", data.ocrType);
    formData.append("postprocessingType", data.postprocessingType);
    formData.append("dbMatchingType", data.dbMatchingType);

    let url = "/api/v1/Run/WithImageUrl";
    let body: any = { ...data };

    const imageFile = data.image?.[0];
    if (imageFile) {
      url = "/api/v1/Run/WithImage";
      formData.append("image", imageFile);
      body = formData;
    } else {
      delete body.image;
    }

    try {
      await axios.post(url, body, {
        headers: {
          "Content-Type": imageFile
            ? "multipart/form-data"
            : "application/json",
        },
      });
      setSuccess("Evaluation run created successfully!");
    } catch (err) {
      setError("Failed to create evaluation run");
    }
    setSubmitting(false);
  };

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-4">Create New Evaluation Run</h1>
      <CreateEvaluationRunForm onSubmit={onSubmit} isSubmitting={submitting} />
      {error && <p className="text-red-500">{error}</p>}
      {success && <p className="text-green-500">{success}</p>}
    </div>
  );
};

export default CreateRunPage;
