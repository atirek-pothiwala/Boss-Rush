#!/usr/bin/env bash
set -euo pipefail

UNITY_VERSION="${UNITY_VERSION:-6000.4.3f1}"
UNITY_IMAGE="${UNITY_IMAGE:-unityci/editor:ubuntu-${UNITY_VERSION}-linux-il2cpp-3}"
WORKSPACE="${GITHUB_WORKSPACE:?}"
UNITY_EMAIL="${UNITY_EMAIL:?}"
UNITY_PASSWORD="${UNITY_PASSWORD:?}"
UNITY_HOME="${WORKSPACE}/.unity-ci-home"

export_unity_license() {
  local ulf_file="$1"
  {
    echo 'UNITY_LICENSE<<UNITY_LICENSE_EOF'
    cat "${ulf_file}"
    echo 'UNITY_LICENSE_EOF'
  } >> "${GITHUB_ENV}"
}

find_ulf_file() {
  find "${UNITY_HOME}" "${WORKSPACE}" -maxdepth 5 -name '*.ulf' -print -quit 2>/dev/null || true
}

if [[ -n "${UNITY_LICENSE:-}" ]]; then
  echo "Using UNITY_LICENSE from repository secrets."
  {
    echo 'UNITY_LICENSE<<UNITY_LICENSE_EOF'
    printf '%s' "${UNITY_LICENSE}"
    echo 'UNITY_LICENSE_EOF'
  } >> "${GITHUB_ENV}"
  exit 0
fi

mkdir -p "${UNITY_HOME}"

echo "Creating Unity manual activation file with ${UNITY_IMAGE}..."
docker run --rm \
  -v "${WORKSPACE}:${WORKSPACE}" \
  -w "${WORKSPACE}" \
  -e HOME="${UNITY_HOME}" \
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

echo "Trying in-container credential activation..."
set +e
docker run --rm \
  -v "${WORKSPACE}:${WORKSPACE}" \
  -w "${WORKSPACE}" \
  -e HOME="${UNITY_HOME}" \
  -e UNITY_EMAIL="${UNITY_EMAIL}" \
  -e UNITY_PASSWORD="${UNITY_PASSWORD}" \
  "${UNITY_IMAGE}" \
  unity-editor \
    -batchmode \
    -quit \
    -logFile /dev/stdout \
    -username "${UNITY_EMAIL}" \
    -password "${UNITY_PASSWORD}"
set -e

ULF_FILE="$(find_ulf_file)"
if [[ -n "${ULF_FILE}" && -f "${ULF_FILE}" ]]; then
  echo "Activated license in-container: ${ULF_FILE}"
  export_unity_license "${ULF_FILE}"
  exit 0
fi

echo "In-container activation did not produce a .ulf file; trying unity-license-activate..."
npm install --global unity-license-activate@0.3.9
pushd "${WORKSPACE}" >/dev/null
if ! unity-license-activate "${UNITY_EMAIL}" "${UNITY_PASSWORD}" "${ALF_FILE}"; then
  if [[ -f error.png ]]; then
    echo "::warning::unity-license-activate failed; uploaded error.png may be available in the job workspace."
  fi
  echo "::error::Could not activate Unity Personal license online."
  echo "Unity's login page may have changed, or 2FA may be required."
  echo "Workaround: add a UNITY_LICENSE secret with a .ulf file from manual activation (see README)."
  exit 1
fi
popd >/dev/null

ULF_FILE="$(find_ulf_file)"
if [[ -z "${ULF_FILE}" || ! -f "${ULF_FILE}" ]]; then
  ULF_FILE=$(find "${WORKSPACE}" -maxdepth 2 -name '*.ulf' -print -quit)
fi
if [[ -z "${ULF_FILE}" || ! -f "${ULF_FILE}" ]]; then
  echo "::error::Failed to obtain Unity license file (.ulf) after online activation."
  exit 1
fi

echo "Created license file: ${ULF_FILE}"
export_unity_license "${ULF_FILE}"
