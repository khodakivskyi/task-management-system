# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy only the .csproj file (for caching)
COPY ["backend.csproj", "./"]

# Restore NuGet packages
# This layer is cached as long as backend.csproj does not change
RUN dotnet restore "backend.csproj"

COPY . .

# Build the project
# -o /app/build = output directory
RUN dotnet build "backend.csproj" -c Release -o /app/build

# ============================================
# Stage 2: Publish
# Create an optimized version for runtime
# ============================================
FROM build AS publish

# Publish the app (produce DLL without extra files)
# /p:UseAppHost=false = do not create a .exe file
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# Stage 3: Runtime
# Final image (small, runtime only)
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Install required libraries for PostgreSQL
RUN apt-get update && \
    apt-get install -y --no-install-recommends libkrb5-3 libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create a non-root user
RUN groupadd --system --gid 1001 appuser && \
    useradd --system --uid 1001 --gid appuser --shell /bin/bash appuser

# Copy published output from the publish stage
COPY --from=publish /app/publish .

# For MigrationRunner
COPY --from=build /src/Infrastructure/Migrations/Scripts /app/Migrations/Scripts

# Copy entrypoint script and prepare app directory
COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh && \
    mkdir -p /app/.aspnet/DataProtection-Keys && \
    chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose application ports
EXPOSE 8080
EXPOSE 8081

# Application entry point
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh", "dotnet", "backend.dll"]
