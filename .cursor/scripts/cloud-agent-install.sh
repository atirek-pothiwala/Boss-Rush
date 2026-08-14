#!/usr/bin/env bash
set -euo pipefail

cd /workspace

git lfs pull

if head -c 8 Assets/Art/Logo.png | grep -q $'^version '; then
  echo "Git LFS assets were not pulled." >&2
  exit 1
fi

mkdir -p /workspace/Logs /workspace/Builds

UNITY_BIN="${UNITY_PATH:-/opt/unity/Editor/Unity}"
if [[ ! -x "$UNITY_BIN" && -x "$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity" ]]; then
  UNITY_BIN="$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity"
fi

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Unity editor not found at UNITY_PATH or the default Hub install path." >&2
  exit 1
fi

if [[ -z "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  echo "Skipping Unity batchmode import: UNITY_ENTITLEMENT_LICENSE is not set." >&2
  exit 0
fi

LICENSE_DIR="${HOME}/.config/unity3d/Unity/licenses"
mkdir -p "${LICENSE_DIR}"
printf '%s' "${UNITY_ENTITLEMENT_LICENSE}" > "${LICENSE_DIR}/UnityEntitlementLicense.xml"

set +e
"$UNITY_BIN" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath /workspace \
  -logFile /workspace/Logs/import.log
import_status=$?
set -e

if [[ "$import_status" -ne 0 ]]; then
  echo "Unity import failed with exit code $import_status." >&2
  exit 1
fi

if grep -E "error CS[0-9]+" /workspace/Logs/import.log; then
  echo "Unity import reported compile errors." >&2
  exit 1
fi
