import { FormProvider, useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import FormInput from "@/core/components/forms/form-input";
import {
  DbMatchingTypeSchema,
  OcrTypeSchema,
  PostprocessingTypeSchema,
  PreprocessingTypeSchema,
} from "@/core/models/run-steps-types";
import FormSelectInput from "@/core/components/forms/form-select-input";
import { Button } from "@/core/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Spinner } from "@/core/components/ui/spinner";
import FormTextArea from "@/core/components/forms/form-text-area";

export const formSchema = z.object({
  runTitle: z.string().min(1, "Title is required"),
  runDescription: z.string().nullish(),
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
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className="w-full flex flex-col items-center"
      >
        <div className="w-full grid grid-cols-1 md:grid-cols-2 gap-4">
          <Card>
            <CardHeader>
              <CardTitle>Run details</CardTitle>
            </CardHeader>
            <CardContent className="flex flex-col gap-3">
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
              <FormTextArea<CreateEvaluationRunFormSchema>
                label="Description"
                name="runDescription"
              />
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Processing steps</CardTitle>
            </CardHeader>
            <CardContent className="flex flex-col gap-3">
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
            </CardContent>
          </Card>
        </div>
        <Button
          type="submit"
          disabled={isSubmitting}
          className="mt-5 w-full md:w-md lg:w-lg"
        >
          {isSubmitting && <Spinner />}
          {isSubmitting ? "Evaluation in progress" : "Run evaluation"}
        </Button>
      </form>
    </FormProvider>
  );
};
