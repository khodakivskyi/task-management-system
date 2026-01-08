#!/bin/sh
set -e

# Fix permissions for DataProtection-Keys directory
# This is needed because Docker volumes are mounted as root
mkdir -p /app/.aspnet/DataProtection-Keys
chown -R appuser:appuser /app/.aspnet/DataProtection-Keys
chmod -R 755 /app/.aspnet/DataProtection-Keys

# Switch to appuser and run the application
CMD="$*"
exec su appuser -c "cd /app && $CMD"
