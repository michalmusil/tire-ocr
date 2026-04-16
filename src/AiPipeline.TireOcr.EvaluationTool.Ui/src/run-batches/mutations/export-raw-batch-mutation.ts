import { useMutation } from "@tanstack/react-query";
import axios from "axios";

const exportRawBatch = async (id: string) => {
  const response = await axios.get(`/api/v1/Batch/${id}/ExportRaw`, {
    responseType: "blob",
  });
  if (response.status !== 200) {
    throw new Error("Failed to export raw evaluation batch");
  }

  return response.data;
};

export const useExportRawBatchMutation = () =>
  useMutation({ mutationFn: exportRawBatch });
