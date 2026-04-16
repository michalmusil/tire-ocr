import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type UpdateBatchProps = {
  batchId: string;
  title?: string | null;
  description?: string | null;
};

const updateBatch = async ({
  batchId,
  title,
  description,
}: UpdateBatchProps) => {
  const request = {
    title: title,
    description: description,
  };
  const response = await axios.put(`/api/v1/Batch/${batchId}`, request);
  if (response.status !== 200) {
    throw new Error(`Failed to update batch ${batchId}`);
  }
};

export const useUpdateBatchMutation = () =>
  useMutation({ mutationFn: updateBatch });
