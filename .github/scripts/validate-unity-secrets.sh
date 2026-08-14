#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_EMAIL:-}" || -z "${UNITY_PASSWORD:-}" ]]; then
  echo "::error::Missing UNITY_EMAIL or UNITY_PASSWORD repository secrets."
  echo "Add them at Settings → Secrets and variables → Actions."
  exit 1
fi

if [[ -z "${UNITY_SERIAL:-}" ]]; then
  echo "::error::Missing UNITY_SERIAL repository secret."
  echo "See README.md → WebGL / GitHub Pages → One-time setup."
  exit 1
fi
