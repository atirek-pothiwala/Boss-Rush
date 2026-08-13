#!/usr/bin/env bash
set -euo pipefail

cd /workspace

mkdir -p /workspace/Logs /workspace/Builds

if command -v git-lfs >/dev/null 2>&1; then
  git lfs pull
fi
