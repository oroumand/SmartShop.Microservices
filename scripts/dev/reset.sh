#!/usr/bin/env bash
set -euo pipefail

if [[ "${1:-}" != "--yes" ]]; then
  echo "This removes the local SmartShop containers and SQL volume."
  echo "Run: $0 --yes"
  exit 2
fi

docker compose down --volumes --remove-orphans
echo "Local SmartShop state was removed."
