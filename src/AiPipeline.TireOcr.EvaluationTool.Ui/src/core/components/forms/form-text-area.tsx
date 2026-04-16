import {
  Controller,
  type FieldValues,
  type Path,
  useFormContext,
} from "react-hook-form";
import { Textarea } from "../ui/textarea";

type FormTextAreaProps<T extends FieldValues> = {
  label: string;
  name: Path<T>;
} & React.TextareaHTMLAttributes<HTMLTextAreaElement>;

const FormTextArea = <T extends FieldValues>({
  label,
  name,
  ...rest
}: FormTextAreaProps<T>) => {
  const { control } = useFormContext<T>();

  return (
    <Controller
      control={control}
      name={name}
      render={({ field, fieldState }) => (
        <>
          <label>
            {label}
            <Textarea
              onChange={field.onChange}
              value={field.value}
              className={`min-h-[120px] ${
                fieldState.error ? "border-red-600" : ""
              }`}
              {...rest}
            />
          </label>
          {fieldState.error && (
            <div>
              <p className="text-red-600">{fieldState.error.message}</p>
            </div>
          )}
        </>
      )}
    />
  );
};

export default FormTextArea;
