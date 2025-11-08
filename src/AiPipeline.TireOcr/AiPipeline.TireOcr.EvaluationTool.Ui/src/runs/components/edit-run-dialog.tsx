import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/core/components/ui/dialog";
import { Button } from "@/core/components/ui/button";
import { DialogClose } from "@/core/components/ui/dialog";
import z from "zod";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/forms/form-input";
import { Spinner } from "@/core/components/ui/spinner";
import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { useUpdateRunMutation } from "../mutations/update-run-mutation";
import { RunDetailQueryKey } from "../queries/use-run-detail-query";

type EditRunDialogProps = {
  evaluationRun: EvaluationRun;
  trigger: React.ReactNode;
};

const formSchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters long"),
});
type EditRunFormSchema = z.infer<typeof formSchema>;

const EditRunDialog = ({ evaluationRun, trigger }: EditRunDialogProps) => {
  const queryClient = useQueryClient();
  const mutation = useUpdateRunMutation();
  const [isOpen, setIsOpen] = useState(false);

  const form = useForm<EditRunFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: evaluationRun.title,
    },
  });

  const onSubmit = (data: EditRunFormSchema) => {
    mutation.mutate(
      {
        runId: evaluationRun.id,
        newTitle: data.title,
      },
      {
        onSuccess: () => {
          setIsOpen(false);
          queryClient.invalidateQueries({
            queryKey: [RunDetailQueryKey, evaluationRun.id],
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
          <DialogTitle>Edit Evaluation Run</DialogTitle>
          <DialogDescription>
            Here you can edit some of the selected run details
          </DialogDescription>
        </DialogHeader>
        <FormProvider {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <div className="flex flex-col gap-2 mb-5">
              <FormInput<EditRunFormSchema>
                label="Run Title"
                name="title"
                placeholder="Title"
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

export default EditRunDialog;
