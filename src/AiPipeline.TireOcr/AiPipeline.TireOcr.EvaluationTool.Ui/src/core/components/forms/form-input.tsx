import { Input } from "@/core/components/ui/input";
import {
  Controller,
  type FieldValues,
  type Path,
  useFormContext,
} from "react-hook-form";

type FormInputProps<T extends FieldValues> = {
  label: string;
  name: Path<T>;
} & React.InputHTMLAttributes<HTMLInputElement>;

const FormInput = <T extends FieldValues>({
  label,
  name,
  ...rest
}: FormInputProps<T>) => {
  const { control } = useFormContext<T>();

  return (
    <Controller
      control={control}
      name={name}
      render={({ field, fieldState }) => (
        <>
          <label>
            <span className="text-muted-foreground">{label}</span>
            <Input
              onChange={(e) => {
                if (name === "") return;
                if (rest.type === "number") {
                  field.onChange(e.target.valueAsNumber);
                  return;
                }
                field.onChange(e.target.value);
              }}
              value={field.value}
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

export default FormInput;
