import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/core/components/ui/dialog";
import {
  CreateEvaluationRunForm,
  type CreateEvaluationRunFormSchema,
} from "../components/create-run-form";
import { useCreateRunMutation } from "../mutations/create-run-mutation";
import { useNavigate } from "react-router-dom";
import { Spinner } from "@/core/components/ui/spinner";
import { useState } from "react";

const CreateRunPage: React.FC = () => {
  const [dialogClosed, setDialogClosed] = useState(false);
  const createRunMutation = useCreateRunMutation();
  const navigate = useNavigate();

  const onSubmit = (data: CreateEvaluationRunFormSchema) => {
    setDialogClosed(false);
    createRunMutation.mutate(data, {
      onSuccess: (id) => {
        navigate(`/runs/${id}`);
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
      <LoadingDialog
        isOpen={createRunMutation.isPending && !dialogClosed}
        onOpenChange={(opened) => setDialogClosed(!opened)}
      />
    </div>
  );
};

const LoadingDialog = ({
  isOpen,
  onOpenChange,
}: {
  isOpen: boolean;
  onOpenChange: (open: boolean) => void;
}) => {
  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent showCloseButton={false}>
        <DialogHeader className="items-center">
          <DialogTitle>Evaluation run is being processed</DialogTitle>
          <Spinner className="m-4 w-12 h-12" />
          <DialogDescription className="text-center">
            The processing may take up to a minute. You can wait here or close
            the dialog and the batch will continue to process in the background.
          </DialogDescription>
        </DialogHeader>
      </DialogContent>
    </Dialog>
  );
};

export default CreateRunPage;
