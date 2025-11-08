import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type UpdateBatchProps = {
  batchId: string;
  newTitle?: string | null;
};

const updateBatch = async ({ batchId, newTitle }: UpdateBatchProps) => {
  const request = {
    batchTitle: newTitle,
  };
  const response = await axios.put(`/api/v1/Batch/${batchId}`, request);
  if (response.status !== 200) {
    throw new Error(`Failed to update batch ${batchId}`);
  }
};

export const useUpdateBatchMutation = () =>
  useMutation({ mutationFn: updateBatch });
