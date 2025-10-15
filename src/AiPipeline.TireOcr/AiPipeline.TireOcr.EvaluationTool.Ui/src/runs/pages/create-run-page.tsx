import {
  CreateEvaluationRunForm,
  type CreateEvaluationRunFormSchema,
} from "../components/create-run-form";
import { useCreateRunMutation } from "../mutations/create-run-mutation";
import { useNavigate } from "react-router-dom";

const CreateRunPage: React.FC = () => {
  const createRunMutation = useCreateRunMutation();
  const navigate = useNavigate();

  const onSubmit = (data: CreateEvaluationRunFormSchema) => {
    createRunMutation.mutate(data, {
      onSuccess: () => {
        navigate(-1);
      },
    });
  };

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-4">Create New Evaluation Run</h1>
      <CreateEvaluationRunForm
        onSubmit={onSubmit}
        isSubmitting={createRunMutation.isPending}
      />
      {createRunMutation.isError && (
        <p className="text-red-500">{createRunMutation.error.message}</p>
      )}
    </div>
  );
};

export default CreateRunPage;
