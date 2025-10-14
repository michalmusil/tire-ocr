import React, { useState } from "react";
import axios from "axios";
import {
  CreateBatchForm,
  type CreateBatchFormSchema,
} from "../components/create-run-batch-form";

const CreateBatchPage: React.FC = () => {
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onSubmit = async (data: CreateBatchFormSchema) => {
    setSubmitting(true);
    setError(null);
    setSuccess(null);

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

    try {
      await axios.post("/api/v1/Batch/Form", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
      setSuccess("Evaluation batch created successfully!");
    } catch (err) {
      setError("Failed to create evaluation batch");
    }
    setSubmitting(false);
  };

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-4">
        Create New Evaluation Batch
      </h1>
      <CreateBatchForm onSubmit={onSubmit} isSubmitting={submitting} />
      {error && <p className="text-red-500">{error}</p>}
      {success && <p className="text-green-500">{success}</p>}
    </div>
  );
};

export default CreateBatchPage;
