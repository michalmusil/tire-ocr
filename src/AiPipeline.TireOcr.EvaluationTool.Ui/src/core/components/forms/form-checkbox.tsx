import { Checkbox } from "@/core/components/ui/checkbox";
import {
  Controller,
  type FieldValues,
  type Path,
  useFormContext,
} from "react-hook-form";

type FormCheckboxProps<T extends FieldValues> = {
  label: string;
  name: Path<T>;
} & React.ComponentPropsWithoutRef<typeof Checkbox>;

const FormCheckbox = <T extends FieldValues>({
  label,
  name,
  ...rest
}: FormCheckboxProps<T>) => {
  const { control } = useFormContext<T>();

  return (
    <Controller
      control={control}
      name={name}
      render={({ field, fieldState }) => (
        <>
          <div className="flex items-center space-x-2">
            <Checkbox
              id={name}
              checked={field.value}
              onCheckedChange={field.onChange}
              className={fieldState.error ? "border-red-600" : ""}
              {...rest}
            />
            <label
              htmlFor={name}
              className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              {label}
            </label>
          </div>
          {fieldState.error && (
            <p className="text-xs text-red-600">{fieldState.error.message}</p>
          )}
        </>
      )}
    />
  );
};

export default FormCheckbox;
