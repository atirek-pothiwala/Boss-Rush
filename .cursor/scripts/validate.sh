#!/usr/bin/env bash
set -euo pipefail

cd /workspace

UNITY_BIN="${UNITY_PATH:-/opt/unity/Editor/Unity}"
if [[ ! -x "$UNITY_BIN" && -x "$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity" ]]; then
  UNITY_BIN="$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity"
fi

mkdir -p /workspace/Logs /workspace/Builds

echo "Using Unity at: $UNITY_BIN"
"$UNITY_BIN" -version

if [[ -z "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  echo "Skipping Unity compile validation: UNITY_ENTITLEMENT_LICENSE is not set."
  echo "Validation succeeded."
  exit 0
fi

LICENSE_DIR="${HOME}/.config/unity3d/Unity/licenses"
mkdir -p "${LICENSE_DIR}"
printf '%s' "${UNITY_ENTITLEMENT_LICENSE}" > "${LICENSE_DIR}/UnityEntitlementLicense.xml"

"$UNITY_BIN" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath /workspace \
  -logFile /workspace/Logs/validate.log

if grep -E "error CS[0-9]+" /workspace/Logs/validate.log; then
  echo "Compile errors found:" >&2
  grep -E "error CS[0-9]+" /workspace/Logs/validate.log >&2
  exit 1
fi

echo "Validation succeeded."
