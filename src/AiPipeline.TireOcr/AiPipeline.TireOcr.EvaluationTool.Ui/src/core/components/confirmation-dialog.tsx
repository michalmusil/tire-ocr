import { Button } from "./ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "./ui/dialog";

type ConfirmationDialogType = "neutral" | "warning" | "destructive";

type ConfirmationDialogProps = {
  title: string;
  type: ConfirmationDialogType;
  description?: string | null;
  confirmText?: string | null;
  cancelText?: string | null;
  onConfirm: () => void;
  onCancel?: () => void;
  trigger: React.ReactNode;
  closeOnConfirm?: boolean | null;
};

const ConfirmationDialog = ({
  title,
  description,
  type,
  confirmText,
  cancelText,
  onConfirm,
  onCancel,
  trigger,
  closeOnConfirm,
}: ConfirmationDialogProps) => {
  const shouldCloseOnConfirm = closeOnConfirm ?? true;

  return (
    <Dialog>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline" onClick={onCancel}>
              {cancelText ?? "Cancel"}
            </Button>
          </DialogClose>
          <ConditionalCloseTrigger shouldClose={() => shouldCloseOnConfirm}>
            <Button
              className={
                type === "destructive"
                  ? "bg-destructive"
                  : type === "warning"
                  ? "bg-amber-500"
                  : "bg-primary"
              }
              onClick={onConfirm}
            >
              {confirmText ?? "Confirm"}
            </Button>
          </ConditionalCloseTrigger>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

const ConditionalCloseTrigger = ({
  children,
  shouldClose,
}: {
  children: React.ReactNode;
  shouldClose: () => boolean;
}) => {
  if (shouldClose()) {
    return <DialogClose asChild>{children}</DialogClose>;
  }
  return children;
};

export default ConfirmationDialog;
