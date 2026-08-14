#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_EMAIL:-}" || -z "${UNITY_PASSWORD:-}" ]]; then
  echo "::error::Missing UNITY_EMAIL or UNITY_PASSWORD repository secrets."
  echo "Add them at Settings → Secrets and variables → Actions."
  exit 1
fi

if [[ -z "${UNITY_SERIAL:-}" && -z "${UNITY_LICENSE:-}" ]]; then
  echo "::error::Missing UNITY_SERIAL (or UNITY_LICENSE)."
  echo "Unity Personal has no serial in your account settings."
  echo "See README.md → WebGL / GitHub Pages → One-time setup for Mac Hub steps."
  exit 1
fi
