#!/usr/bin/env bash
set -euo pipefail

cd /workspace

git lfs pull

mkdir -p /workspace/Logs /workspace/Builds

UNITY_BIN="${UNITY_PATH:-/opt/unity/Editor/Unity}"
if [[ ! -x "$UNITY_BIN" && -x "$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity" ]]; then
  UNITY_BIN="$HOME/Unity/Hub/Editor/6000.4.3f1/Editor/Unity"
fi

if [[ ! -x "$UNITY_BIN" ]]; then
  echo "Unity editor not found at UNITY_PATH or the default Hub install path." >&2
  exit 1
fi

has_license=false
if [[ -n "${UNITY_LICENSE:-}" ]]; then
  mkdir -p "$HOME/.local/share/unity3d/Unity"
  printf '%s' "$UNITY_LICENSE" > "$HOME/.local/share/unity3d/Unity/Unity_lic.ulf"
  has_license=true
elif [[ -n "${UNITY_EMAIL:-}" && -n "${UNITY_PASSWORD:-}" ]]; then
  serial_args=()
  if [[ -n "${UNITY_SERIAL:-}" ]]; then
    serial_args=(-serial "$UNITY_SERIAL")
  else
    serial_args=(-serial)
  fi

  "$UNITY_BIN" \
    -batchmode \
    -nographics \
    -quit \
    "${serial_args[@]}" \
    -username "$UNITY_EMAIL" \
    -password "$UNITY_PASSWORD" \
    -logFile /workspace/Logs/license-activation.log
  has_license=true
fi

if [[ "$has_license" == true ]]; then
  "$UNITY_BIN" \
    -batchmode \
    -nographics \
    -quit \
    -projectPath /workspace \
    -logFile /workspace/Logs/import.log

  if grep -E "error CS[0-9]+" /workspace/Logs/import.log; then
    echo "Unity import reported compile errors." >&2
    exit 1
  fi
else
  echo "Skipping Unity batchmode import: no license credentials configured."
  echo "Add UNITY_LICENSE or UNITY_EMAIL/UNITY_PASSWORD to enable compile validation."
fi
