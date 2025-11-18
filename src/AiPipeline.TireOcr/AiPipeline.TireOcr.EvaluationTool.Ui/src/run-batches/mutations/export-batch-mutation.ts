import { useMutation } from "@tanstack/react-query";
import axios from "axios";

const exportBatch = async (id: string) => {
  const response = await axios.get(`/api/v1/Batch/${id}/Export`, {
    responseType: "blob",
  });
  if (response.status !== 200) {
    throw new Error("Failed to export an evaluation batch");
  }

  return response.data;
};

export const useExportBatchMutation = () =>
  useMutation({ mutationFn: exportBatch });
