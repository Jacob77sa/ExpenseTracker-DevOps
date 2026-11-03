#!/usr/bin/env bash
set -euo pipefail

PROD=production
APP=build/app
ATTEMPTS="${CHECK_ATTEMPTS:-20}"
mkdir -p "$PROD"

CURRENT=""
if [ -L "$PROD/current" ]; then CURRENT="$(readlink "$PROD/current")"; fi
if [ "$CURRENT" = "blue" ]; then SLOT=green; PORT=5002; else SLOT=blue; PORT=5001; fi

stop_slot() {
  if [ -f "$PROD/$1.pid" ]; then
    kill "$(cat "$PROD/$1.pid")" 2>/dev/null || true
    rm -f "$PROD/$1.pid"
  fi
}

stop_slot "$SLOT"
rm -rf "$PROD/$SLOT"
mkdir -p "$PROD/$SLOT"
cp -r "$APP"/. "$PROD/$SLOT/"

if [ "${INJECT_FAILURE:-0}" = "1" ]; then
  echo "Simulating a crashing release in slot $SLOT"
else
  ASPNETCORE_URLS="http://127.0.0.1:$PORT" nohup dotnet "$PROD/$SLOT/ExpenseTracker.dll" > "$PROD/$SLOT.log" 2>&1 &
  echo $! > "$PROD/$SLOT.pid"
fi

healthy=0
for _ in $(seq 1 "$ATTEMPTS"); do
  if curl -fsS "http://127.0.0.1:$PORT/health" > /dev/null 2>&1; then healthy=1; break; fi
  sleep 1
done

if [ "$healthy" -ne 1 ]; then
  echo "$(date -u +%FT%TZ) ROLLBACK reason=unhealthy-candidate slot=$SLOT" | tee -a "$PROD/monitoring.log"
  stop_slot "$SLOT"
  exit 1
fi

ln -sfn "$SLOT" "$PROD/current"
echo "$PORT" > "$PROD/current.port"
if [ -n "$CURRENT" ]; then stop_slot "$CURRENT"; fi
echo "$(date -u +%FT%TZ) DEPLOYED slot=$SLOT port=$PORT" | tee -a "$PROD/monitoring.log"
