#!/usr/bin/env bash
set -euo pipefail

UNITY_VERSION="${UNITY_VERSION:-6000.4.3f1}"
UNITY_IMAGE="${UNITY_IMAGE:-unityci/editor:ubuntu-${UNITY_VERSION}-linux-il2cpp-3}"
WORKSPACE="${GITHUB_WORKSPACE:?}"
UNITY_EMAIL="${UNITY_EMAIL:?}"
UNITY_PASSWORD="${UNITY_PASSWORD:?}"

echo "Creating Unity manual activation file with ${UNITY_IMAGE}..."
docker run --rm \
  -v "${WORKSPACE}:${WORKSPACE}" \
  -w "${WORKSPACE}" \
  -e HOME="${WORKSPACE}" \
  "${UNITY_IMAGE}" \
  unity-editor \
    -batchmode \
    -quit \
    -logFile /dev/stdout \
    -createManualActivationFile

ALF_FILE=$(find "${WORKSPACE}" -maxdepth 2 -name 'Unity_v*.alf' -print -quit)
if [[ -z "${ALF_FILE}" || ! -f "${ALF_FILE}" ]]; then
  echo "::error::Failed to create Unity activation file (.alf)."
  exit 1
fi
echo "Created activation file: ${ALF_FILE}"

echo "Activating Unity Personal license online..."
npm install --global unity-license-activate@0.3.9
pushd "${WORKSPACE}" >/dev/null
unity-license-activate "${UNITY_EMAIL}" "${UNITY_PASSWORD}" "${ALF_FILE}"
popd >/dev/null

ULF_FILE=$(find "${WORKSPACE}" -maxdepth 2 -name '*.ulf' -print -quit)
if [[ -z "${ULF_FILE}" || ! -f "${ULF_FILE}" ]]; then
  echo "::error::Failed to obtain Unity license file (.ulf) after online activation."
  exit 1
fi
echo "Created license file: ${ULF_FILE}"

{
  echo 'UNITY_LICENSE<<UNITY_LICENSE_EOF'
  cat "${ULF_FILE}"
  echo 'UNITY_LICENSE_EOF'
} >> "${GITHUB_ENV}"
