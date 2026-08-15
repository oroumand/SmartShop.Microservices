#!/usr/bin/env bash
set -euo pipefail

command -v docker >/dev/null || { echo "docker is required" >&2; exit 1; }
command -v curl >/dev/null || { echo "curl is required" >&2; exit 1; }

docker compose up --build -d

for url in \
  http://localhost:8080/health/ready \
  http://localhost:8081/health/ready \
  http://localhost:8082/health/ready \
  http://localhost:8088/health/ready
do
  printf 'waiting for %s\n' "$url"
  for _ in $(seq 1 60); do
    if curl --silent --fail "$url" >/dev/null; then
      break
    fi
    sleep 2
  done
  curl --silent --fail "$url" >/dev/null
done

docker compose ps
echo "SmartShop is ready at http://localhost:8088"
