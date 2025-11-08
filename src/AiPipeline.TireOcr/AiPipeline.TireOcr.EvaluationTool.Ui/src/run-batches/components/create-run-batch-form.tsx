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
import FormFileInput from "@/core/components/forms/form-file-input";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Spinner } from "@/core/components/ui/spinner";

export const formSchema = z.object({
  runTitle: z.string().nullish(),
  imageUrlsWithExpectedTireCodeLabelsCsv: z
    .instanceof(FileList)
    .refine(
      (files) =>
        files.length === 1 && files[0].type.toLowerCase() === "text/csv",
      "Exactly one CSV file should be supplied."
    ),
  processingBatchSize: z
    .number()
    .int()
    .refine(
      (n) => n > 0 && n <= 10,
      "Processing batch size must be between 1 and 10 for performance reasons."
    ),
  preprocessingType: PreprocessingTypeSchema,
  ocrType: OcrTypeSchema,
  postprocessingType: PostprocessingTypeSchema,
  dbMatchingType: DbMatchingTypeSchema,
});

export type CreateBatchFormSchema = z.infer<typeof formSchema>;

type CreateBatchFormProps = {
  onSubmit: (data: CreateBatchFormSchema) => void;
  isSubmitting: boolean;
};

export const CreateBatchForm = ({
  onSubmit,
  isSubmitting,
}: CreateBatchFormProps) => {
  const form = useForm<CreateBatchFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      processingBatchSize: 5,
    },
  });

  return (
    <FormProvider {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className="w-full flex flex-col items-center"
      >
        <div className="w-full grid grid-cols-1 md:grid-cols-2 gap-4">
          <Card className="flex-1">
            <CardHeader>
              <CardTitle>Batch details</CardTitle>
            </CardHeader>
            <CardContent>
              <FormInput<CreateBatchFormSchema> label="Title" name="runTitle" />
              <FormInput<CreateBatchFormSchema>
                label="Processing batch size"
                name="processingBatchSize"
                type="number"
              />
              <FormFileInput<CreateBatchFormSchema>
                label="Expected tire code"
                name="imageUrlsWithExpectedTireCodeLabelsCsv"
                accept="text/csv"
              />
            </CardContent>
          </Card>
          <Card className="flex-1">
            <CardHeader>
              <CardTitle>Processing steps</CardTitle>
            </CardHeader>
            <CardContent>
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
          {isSubmitting ? "Processing in progress" : "Start processing"}
        </Button>
      </form>
    </FormProvider>
  );
};
