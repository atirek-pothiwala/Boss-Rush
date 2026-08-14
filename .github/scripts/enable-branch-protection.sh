#!/usr/bin/env bash
set -euo pipefail

# Enables branch protection on main for Boss Rush.
# Requires: gh CLI authenticated as a repo admin/owner.
# Usage: bash .github/scripts/enable-branch-protection.sh

REPO="${1:-atirek-pothiwala/Boss-Rush}"
BRANCH="${2:-main}"
CI_CHECK="Compile validation"

echo "Enabling branch protection on ${REPO}:${BRANCH}"

gh api --method PUT "repos/${REPO}/branches/${BRANCH}/protection" --input - <<EOF
{
  "required_status_checks": {
    "strict": true,
    "checks": [
      {
        "context": "${CI_CHECK}"
      }
    ]
  },
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "required_approving_review_count": 0,
    "dismiss_stale_reviews": true
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": false
}
EOF

echo "Branch protection enabled."
echo "  - Pull requests required before merging to ${BRANCH}"
echo "  - Required status check: ${CI_CHECK}"
echo "  - Force pushes and branch deletion blocked"
