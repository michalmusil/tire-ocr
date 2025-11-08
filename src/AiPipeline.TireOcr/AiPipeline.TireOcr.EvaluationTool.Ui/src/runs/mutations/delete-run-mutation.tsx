import { useMutation } from "@tanstack/react-query";
import axios from "axios";

const deleteRun = async (id: string) => {
  const response = await axios.delete(`/api/v1/Run/${id}`);
  if (response.status !== 200 && response.status !== 204) {
    throw new Error("Failed to delete an evaluation run");
  }
};

export const useDeleteRunMutation = () =>
  useMutation({ mutationFn: deleteRun });
