#!/usr/bin/env bash
for pidfile in production/*.pid; do
  [ -f "$pidfile" ] || continue
  kill "$(cat "$pidfile")" 2>/dev/null || true
  rm -f "$pidfile"
done
echo "Application stopped."
