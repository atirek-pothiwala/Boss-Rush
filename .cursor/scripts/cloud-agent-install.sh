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

  set +e
  "$UNITY_BIN" \
    -batchmode \
    -nographics \
    -quit \
    "${serial_args[@]}" \
    -username "$UNITY_EMAIL" \
    -password "$UNITY_PASSWORD" \
    -logFile /workspace/Logs/license-activation.log
  activation_status=$?
  set -e

  if [[ "$activation_status" -eq 0 ]] \
    && ! grep -q "No valid Unity Editor license found" /workspace/Logs/license-activation.log; then
    has_license=true
  else
    echo "Unity license activation unavailable; continuing without batchmode import." >&2
    echo "See /workspace/Logs/license-activation.log for details." >&2
  fi
fi

if [[ "$has_license" == true ]]; then
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
else
  echo "Skipping Unity batchmode import: no usable Unity license configured."
  echo "Add UNITY_LICENSE or valid UNITY_EMAIL/UNITY_PASSWORD to enable compile validation."
fi
