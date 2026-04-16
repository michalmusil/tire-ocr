import { Search, X } from "lucide-react";
import { Input } from "@/core/components/ui/input";
import { Button } from "@/core/components/ui/button";
import { useRef, useState } from "react";

type SearchInputProps = {
  onSearchChanged: (searchString: string | null) => void;
  initialValue?: string | null;
  placeholder?: string | null;
} & Omit<React.InputHTMLAttributes<HTMLInputElement>, "type">;

function SearchInput({
  onSearchChanged,
  initialValue,
  placeholder,
  ...rest
}: SearchInputProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [isEmpty, setIsEmpty] = useState<boolean>(
    initialValue === undefined || initialValue === null || initialValue === ""
  );

  const handleSearch = (value: string) => {
    if (value.length === 0) {
      onSearchChanged(null);
    } else {
      onSearchChanged(value);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    const currentValue = e.currentTarget.value;
    if (e.key === "Enter") {
      handleSearch(currentValue);
    }
    rest.onKeyDown?.(e);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const currentValue = e.currentTarget.value;
    setIsEmpty(currentValue.length === 0);
    rest.onChange?.(e);
  };

  const handleClear = () => {
    if (inputRef.current) {
      inputRef.current.value = "";
    }
    setIsEmpty(true);
    onSearchChanged(null);
  };

  return (
    <div className="flex w-full items-center gap-2">
      <div className="relative flex-1">
        <Input
          ref={inputRef}
          type="text"
          placeholder={placeholder ?? "Search..."}
          defaultValue={initialValue ?? ""}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          {...rest}
        />
      </div>

      <Button
        onClick={() => handleSearch(inputRef.current?.value ?? "")}
        size="icon"
        variant="default"
      >
        <Search />
      </Button>

      <Button
        onClick={handleClear}
        size="icon"
        variant="outline"
        className={`transition-opacity duration-200 ${
          isEmpty ? "opacity-0" : "opacity-100"
        }`}
      >
        <X />
      </Button>
    </div>
  );
}

export default SearchInput;
