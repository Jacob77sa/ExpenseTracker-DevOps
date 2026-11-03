#!/usr/bin/env bash
set -euo pipefail
dotnet test ExpenseTracker.sln -c Release --verbosity normal
