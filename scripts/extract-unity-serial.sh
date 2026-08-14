#!/usr/bin/env bash
# Extract the activation serial GameCI expects from a Hub-generated Unity_lic.ulf file.
set -euo pipefail

ULF_FILE="${1:-}"
if [[ -z "${ULF_FILE}" ]]; then
  for candidate in \
    "/Library/Application Support/Unity/Unity_lic.ulf" \
    "${HOME}/Library/Application Support/Unity/Unity_lic.ulf"; do
    if [[ -f "${candidate}" ]]; then
      ULF_FILE="${candidate}"
      break
    fi
  done
fi

if [[ -z "${ULF_FILE}" || ! -f "${ULF_FILE}" ]]; then
  echo "Could not find Unity_lic.ulf." >&2
  echo "Try: find /Library ~/Library -name 'Unity_lic.ulf' 2>/dev/null" >&2
  exit 1
fi

python3 - "${ULF_FILE}" <<'PY'
import base64
import re
import sys

ulf = open(sys.argv[1], encoding="utf-8", errors="ignore").read()
match = re.search(r'<DeveloperData Value="([^"]+)"', ulf)
if not match:
    raise SystemExit("DeveloperData not found in Unity_lic.ulf")

serial = base64.b64decode(match.group(1))[4:].decode("latin-1")
print(serial)
PY
