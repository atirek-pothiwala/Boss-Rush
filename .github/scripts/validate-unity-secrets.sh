#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_EMAIL:-}" || -z "${UNITY_PASSWORD:-}" ]]; then
  echo "::error::Missing UNITY_EMAIL or UNITY_PASSWORD repository secrets."
  echo "Add them at Settings → Secrets and variables → Actions."
  exit 1
fi

if [[ -n "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  exit 0
fi

echo "::error::Missing UNITY_ENTITLEMENT_LICENSE repository secret."
echo "Copy the full XML from ~/Library/Unity/licenses/UnityEntitlementLicense.xml."
echo "See README.md → WebGL / GitHub Pages → One-time setup."
exit 1
