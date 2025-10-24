import {
  CreateBatchForm,
  type CreateBatchFormSchema,
} from "../components/create-run-batch-form";
import { useCreateBatchMutation } from "../mutations/create-batch-mutation";
import { useNavigate } from "react-router-dom";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/core/components/ui/dialog";
import { DialogHeader } from "@/core/components/ui/dialog";
import { Spinner } from "@/core/components/ui/spinner";
import { useState } from "react";

const CreateBatchPage: React.FC = () => {
  const [dialogClosed, setDialogClosed] = useState(false);
  const createBatchMutation = useCreateBatchMutation();
  const navigate = useNavigate();

  const onSubmit = (data: CreateBatchFormSchema) => {
    setDialogClosed(true);
    createBatchMutation.mutate(data, {
      onSuccess: (id) => {
        navigate(`/batches/${id}`);
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
      <LoadingDialog
        isOpen={createBatchMutation.isPending && !dialogClosed}
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
      <DialogContent>
        <DialogHeader className="items-center">
          <DialogTitle>Batch is being processed</DialogTitle>
          <Spinner className="m-4 w-12 h-12" />
          <DialogDescription className="text-center">
            The processing may take a long time (depending on number of images).
            You can wait here or close the dialog and the batch will continue to
            process in the background.
          </DialogDescription>
        </DialogHeader>
      </DialogContent>
    </Dialog>
  );
};

export default CreateBatchPage;
