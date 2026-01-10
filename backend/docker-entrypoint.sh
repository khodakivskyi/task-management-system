#!/bin/sh
set -e

# Fix permissions for DataProtection-Keys directory
mkdir -p /app/.aspnet/DataProtection-Keys
chown -R appuser:appuser /app/.aspnet/DataProtection-Keys
chmod -R 755 /app/.aspnet/DataProtection-Keys

# Run the application
exec "$@"
