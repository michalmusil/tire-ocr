export const getDisplayedDurationFromDatetimeBoundaries = (
  startedAtStr?: string | null,
  endedAtStr?: string | null
) => {
  const startedAt = startedAtStr ? new Date(startedAtStr) : null;
  const endedAt = endedAtStr ? new Date(endedAtStr) : null;
  const duration =
    startedAt && endedAt ? endedAt.getTime() - startedAt.getTime() : null;

  const durationInSeconds = duration ? duration / 1000 : null;
  const displayedDuration = durationInSeconds
    ? durationInSeconds > 60
      ? `${(durationInSeconds / 60).toFixed(0)}min ${(
          durationInSeconds % 60
        ).toFixed(0)}s`
      : `${durationInSeconds.toFixed(2)}s`
    : "-";

  return displayedDuration;
};

export const getDisplayedDurationFromMs = (milliseconds?: number | null) => {
  const durationInSeconds = milliseconds ? milliseconds / 1000 : null;
  const displayedDuration = durationInSeconds
    ? durationInSeconds > 60
      ? `${(durationInSeconds / 60).toFixed(0)}min ${(
          durationInSeconds % 60
        ).toFixed(0)}s`
      : `${durationInSeconds.toFixed(2)}s`
    : "-";

  return displayedDuration;
};
