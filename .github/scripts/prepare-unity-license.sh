#!/usr/bin/env bash
set -euo pipefail

GITHUB_OUTPUT="${GITHUB_OUTPUT:?}"

if [[ -z "${UNITY_ENTITLEMENT_LICENSE:-}" ]]; then
  echo "::error::No license material available after validation."
  exit 1
fi

LICENSE_DIR="${RUNNER_TEMP}/_github_home/.config/unity3d/Unity/licenses"
mkdir -p "${LICENSE_DIR}"
printf '%s' "${UNITY_ENTITLEMENT_LICENSE}" > "${LICENSE_DIR}/UnityEntitlementLicense.xml"
echo "Installed UnityEntitlementLicense.xml into the GameCI docker home directory."
echo "skip_activation=true" >> "${GITHUB_OUTPUT}"
