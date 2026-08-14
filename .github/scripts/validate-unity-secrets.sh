#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_EMAIL:-}" || -z "${UNITY_PASSWORD:-}" ]]; then
  echo "::error::Missing UNITY_EMAIL or UNITY_PASSWORD repository secrets."
  echo "Add them at Settings → Secrets and variables → Actions."
  exit 1
fi

if [[ -n "${UNITY_SERIAL:-}" ]]; then
  exit 0
fi

if [[ -n "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  exit 0
fi

echo "::error::Missing a Unity license secret."
echo "Add UNITY_SERIAL (recommended) or UNITY_ENTITLEMENT_LICENSE (Hub XML fallback)."
echo "See README.md → WebGL / GitHub Pages → One-time setup."
exit 1
