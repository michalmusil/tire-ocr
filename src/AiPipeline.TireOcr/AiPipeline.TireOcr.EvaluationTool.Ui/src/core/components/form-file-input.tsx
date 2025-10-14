import { Input } from "@/core/components/ui/input";
import {
  Controller,
  type FieldValues,
  type Path,
  useFormContext,
} from "react-hook-form";

type FormFileInputProps<T extends FieldValues> = {
  label: string;
  accept?: string;
  name: Path<T>;
} & React.InputHTMLAttributes<HTMLInputElement>;

const FormFileInput = <T extends FieldValues>({
  label,
  name,
  accept,
  ...rest
}: FormFileInputProps<T>) => {
  const { control } = useFormContext<T>();

  return (
    <Controller
      control={control}
      name={name}
      render={({ field, fieldState }) => (
        <>
          <label>
            {label}

            <Input
              type="file"
              accept={accept}
              onChange={(e) => {
                if (name === "" || e.target.files === null) return;
                field.onChange(e.target.files);
              }}
              className={fieldState.error ? "border-red-600" : ""}
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

export default FormFileInput;
