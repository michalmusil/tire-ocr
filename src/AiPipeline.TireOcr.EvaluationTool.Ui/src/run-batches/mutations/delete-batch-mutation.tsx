import { useMutation } from "@tanstack/react-query";
import axios from "axios";

const deleteBatch = async (id: string) => {
  const response = await axios.delete(`/api/v1/Batch/${id}`);
  if (response.status !== 200 && response.status !== 204) {
    throw new Error("Failed to delete an evaluation batch");
  }
};

export const useDeleteBatchMutation = () =>
  useMutation({ mutationFn: deleteBatch });
