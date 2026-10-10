#!/usr/bin/env bash
# Starts a program with a window and fails unless it is still running after <seconds>: a crash at start-up fails, a
# window that stays open passes. The program is stopped afterwards; when it fails, its output is shown.
set -euo pipefail

usage="usage: check-starts.sh <seconds> <program> [arguments]"
seconds="${1:?$usage}"
shift
[[ $# -gt 0 ]] || { echo "$usage" >&2; exit 1; }

log="$(mktemp)"
"$@" > "$log" 2>&1 &
pid=$!
for _ in $(seq "$seconds"); do
  if ! kill -0 "$pid" 2> /dev/null; then
    wait "$pid" && status=0 || status=$?
    echo "$1 exited with $status within $seconds seconds:" >&2
    cat "$log" >&2
    exit 1
  fi
  sleep 1
done
# No wait: Git Bash on Windows may not be able to stop a native program, and its output goes to the log, so a program
# that outlives the script holds up nothing.
kill "$pid" 2> /dev/null || true
for _ in $(seq 10); do
  kill -0 "$pid" 2> /dev/null || break
  sleep 1
done
kill -9 "$pid" 2> /dev/null || true
echo "$1 is still running after $seconds seconds."
