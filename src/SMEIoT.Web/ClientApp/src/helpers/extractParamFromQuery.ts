import { WindowLocation } from "@reach/router";

export default function extractParamFromQuery(location: WindowLocation | undefined): string | null {
  if (!location) {
    return null;
  }
  const params = new URLSearchParams(location.search.slice(1));
  const name = params.get("name");
  return name === null ? null : name;
}