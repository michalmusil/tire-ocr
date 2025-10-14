import { FormProvider, useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/form-input";
import {
  DbMatchingTypeSchema,
  OcrTypeSchema,
  PostprocessingTypeSchema,
  PreprocessingTypeSchema,
} from "@/core/dtos/run-steps-types";
import FormSelectInput from "@/core/components/form-select-input";
import { Button } from "@/core/components/ui/button";

export const formSchema = z.object({
  runTitle: z.string().min(1, "Title is required"),
  imageUrl: z.url().nullish(),
  image: z.instanceof(FileList).nullish(),
  expectedTireCodeLabel: z.string().nullish(),
  preprocessingType: PreprocessingTypeSchema,
  ocrType: OcrTypeSchema,
  postprocessingType: PostprocessingTypeSchema,
  dbMatchingType: DbMatchingTypeSchema,
});

export type CreateEvaluationRunFormSchema = z.infer<typeof formSchema>;

type CreateRunFormProps = {
  onSubmit: (data: CreateEvaluationRunFormSchema) => void;
  isSubmitting: boolean;
};

export const CreateEvaluationRunForm = ({
  onSubmit,
  isSubmitting,
}: CreateRunFormProps) => {
  const form = useForm<CreateEvaluationRunFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      preprocessingType: "Resize",
    },
  });

  return (
    <FormProvider {...form}>
      <div className="max-w-screen-sm flex-col">
        <form onSubmit={form.handleSubmit(onSubmit)}>
          <FormInput<CreateEvaluationRunFormSchema>
            label="Title"
            name="runTitle"
          />
          <FormInput<CreateEvaluationRunFormSchema>
            label="Image URL"
            name="imageUrl"
          />
          <FormInput<CreateEvaluationRunFormSchema>
            label="Expected tire code"
            name="expectedTireCodeLabel"
          />
          <FormSelectInput
            label="Preprocessing"
            name="preprocessingType"
            options={PreprocessingTypeSchema.options}
          />
          <FormSelectInput
            label="Ocr"
            name="ocrType"
            options={OcrTypeSchema.options}
          />
          <FormSelectInput
            label="Postprocessing"
            name="postprocessingType"
            options={PostprocessingTypeSchema.options}
          />
          <FormSelectInput
            label="DB matching"
            name="dbMatchingType"
            options={DbMatchingTypeSchema.options}
          />
          <Button type="submit" disabled={isSubmitting}>
            Create
          </Button>
        </form>
      </div>
    </FormProvider>
  );
};
