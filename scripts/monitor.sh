#!/usr/bin/env bash
set -uo pipefail

PROD=production
LOG="$PROD/monitoring.log"
mkdir -p "$PROD"

check() {
  local port ts code
  port="$(cat "$PROD/current.port" 2>/dev/null || echo 5001)"
  ts="$(date -u +%FT%TZ)"
  code="$(curl -s -o /dev/null -w '%{http_code}' "http://127.0.0.1:$port/health" || true)"
  if [ "$code" = "200" ]; then
    echo "$ts HEALTHY port=$port http=$code" | tee -a "$LOG"
  else
    echo "$ts UNHEALTHY port=$port http=$code" | tee -a "$LOG"
    return 1
  fi
}

if [ "${1:-}" = "--once" ]; then
  check
else
  while true; do check || true; sleep 5; done
fi
