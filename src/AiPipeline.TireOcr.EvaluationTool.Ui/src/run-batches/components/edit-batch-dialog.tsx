import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/core/components/ui/dialog";
import type { RunBatchDetail } from "../dtos/get-run-batch-detail-dto";
import { Button } from "@/core/components/ui/button";
import { DialogClose } from "@/core/components/ui/dialog";
import z from "zod";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/forms/form-input";
import { useUpdateBatchMutation } from "../mutations/update-batch-mutation";
import { Spinner } from "@/core/components/ui/spinner";
import { useQueryClient } from "@tanstack/react-query";
import { RunBatchDetailQueryKey } from "../queries/use-run-batch-detail-query";
import { useState } from "react";
import FormTextArea from "@/core/components/forms/form-text-area";

type EditBatchDialogProps = {
  batch: RunBatchDetail;
  trigger: React.ReactNode;
};

const formSchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters long"),
  description: z.string().nullish(),
});
type EditBatchFormSchema = z.infer<typeof formSchema>;

const EditBatchDialog = ({ batch, trigger }: EditBatchDialogProps) => {
  const queryClient = useQueryClient();
  const mutation = useUpdateBatchMutation();
  const [isOpen, setIsOpen] = useState(false);

  const form = useForm<EditBatchFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: batch.title,
      description: batch.description,
    },
  });

  const onSubmit = (data: EditBatchFormSchema) => {
    mutation.mutate(
      {
        batchId: batch.id,
        title: data.title,
        description: data.description,
      },
      {
        onSuccess: () => {
          setIsOpen(false);
          queryClient.invalidateQueries({
            queryKey: [RunBatchDetailQueryKey, batch.id],
          });
        },
        onError: (error) => {
          console.log(error);
        },
      }
    );
  };

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Evaluation Batch</DialogTitle>
          <DialogDescription>
            Here you can edit some of the selected batch details
          </DialogDescription>
        </DialogHeader>
        <FormProvider {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <div className="flex flex-col gap-2 mb-5">
              <FormInput<EditBatchFormSchema>
                label="Batch Title"
                name="title"
                placeholder="Title"
              />

              <FormTextArea<EditBatchFormSchema>
                label="Batch Description"
                name="description"
                placeholder="Description"
              />

              {mutation.error?.message && (
                <div className="mt-2 text-sm text-destructive">
                  {mutation.error?.message}
                </div>
              )}
            </div>
            <DialogFooter>
              <DialogClose asChild>
                <Button variant="outline">Cancel</Button>
              </DialogClose>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending && <Spinner />}
                {mutation.isPending ? "Updating..." : "Update"}
              </Button>
            </DialogFooter>
          </form>
        </FormProvider>
      </DialogContent>
    </Dialog>
  );
};

export default EditBatchDialog;
