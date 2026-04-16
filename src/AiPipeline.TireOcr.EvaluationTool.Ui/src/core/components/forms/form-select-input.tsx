import { Controller, useFormContext } from "react-hook-form";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/core/components/ui/select";

type FormSelectFieldProps = {
  name: string;
  label: string;
  options: string[];
  placeholder?: string;
};

const FormSelectInput = ({
  name,
  label,
  options,
  placeholder = "Select an option",
}: FormSelectFieldProps) => {
  const {
    control,
    formState: { errors },
  } = useFormContext();
  return (
    <div className="form-group">
      <label className="block text-sm font-medium mb-1">{label}</label>
      <Controller
        name={name}
        control={control}
        render={({ field: { value, name, onChange, disabled } }) => (
          <Select
            onValueChange={onChange}
            value={value}
            name={name}
            disabled={disabled}
          >
            <SelectTrigger>
              <SelectValue placeholder={placeholder} />
            </SelectTrigger>
            <SelectContent>
              {options.map((option) => (
                <SelectItem key={option} value={option}>
                  {option}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        )}
      />
      {errors[name] && (
        <p className="text-red-500 text-sm">
          {(errors[name] as { message?: string }).message ??
            "This field is required."}
        </p>
      )}
    </div>
  );
};

export default FormSelectInput;
