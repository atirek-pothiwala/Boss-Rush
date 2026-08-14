#!/usr/bin/env bash
set -euo pipefail

GITHUB_OUTPUT="${GITHUB_OUTPUT:?}"

if [[ -n "${UNITY_SERIAL:-}" ]]; then
  echo "Using serial license activation."
  echo "skip_activation=false" >> "${GITHUB_OUTPUT}"
  echo "use_entitlement=false" >> "${GITHUB_OUTPUT}"
  exit 0
fi

if [[ -z "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  echo "::error::No license material available after validation."
  exit 1
fi

LICENSE_DIR="${RUNNER_TEMP}/_github_home/.config/unity3d/Unity/licenses"
mkdir -p "${LICENSE_DIR}"
printf '%s' "${UNITY_ENTITLEMENT_LICENSE}" > "${LICENSE_DIR}/UnityEntitlementLicense.xml"
echo "Installed UnityEntitlementLicense.xml into the GameCI docker home directory."
echo "skip_activation=true" >> "${GITHUB_OUTPUT}"
echo "use_entitlement=true" >> "${GITHUB_OUTPUT}"
