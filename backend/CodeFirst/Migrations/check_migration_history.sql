SELECT 
    "MigrationId",
    "ProductVersion",
    "AppliedAt" = NOW()
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

