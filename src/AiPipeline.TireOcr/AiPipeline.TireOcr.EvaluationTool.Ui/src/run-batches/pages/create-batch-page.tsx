import {
  CreateBatchForm,
  type CreateBatchFormSchema,
} from "../components/create-run-batch-form";
import { useCreateBatchMutation } from "../mutations/create-batch-mutation";
import { useNavigate } from "react-router-dom";

const CreateBatchPage: React.FC = () => {
  const createBatchMutation = useCreateBatchMutation();
  const navigate = useNavigate();

  const onSubmit = (data: CreateBatchFormSchema) => {
    createBatchMutation.mutate(data, {
      onSuccess: () => {
        navigate(-1);
      },
    });
  };

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-4">
        Create New Evaluation Batch
      </h1>
      <CreateBatchForm
        onSubmit={onSubmit}
        isSubmitting={createBatchMutation.isPending}
      />
      {createBatchMutation.isError && (
        <p className="text-red-500">{createBatchMutation.error.message}</p>
      )}
    </div>
  );
};

export default CreateBatchPage;
