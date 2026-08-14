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

has_license=false
if [[ -n "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  LICENSE_DIR="${HOME}/.config/unity3d/Unity/licenses"
  mkdir -p "${LICENSE_DIR}"
  printf '%s' "${UNITY_ENTITLEMENT_LICENSE}" > "${LICENSE_DIR}/UnityEntitlementLicense.xml"
  has_license=true
elif [[ -n "${UNITY_EMAIL:-}" && -n "${UNITY_PASSWORD:-}" && -n "${UNITY_SERIAL:-}" ]]; then
  has_license=true
fi

if [[ "$has_license" == true ]]; then
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
else
  echo "Skipping Unity compile validation: no license credentials configured."
fi

echo "Validation succeeded."
