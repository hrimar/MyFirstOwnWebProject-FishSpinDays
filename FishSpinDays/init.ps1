# Read .env file
$envVars = Get-Content ".env" | Where-Object { $_ -match "=" } | ForEach-Object {
    $parts = $_ -split "="
    @{ Name = $parts[0].Trim(); Value = $parts[1].Trim() }
}

# Create hash table for easy access
$envHash = @{}
foreach ($var in $envVars) {
    $envHash[$var.Name] = $var.Value
}

# Generate init.sql with environment values and proper line breaks
$initSql = @()
$initSql += "RAISERROR('Init script started', 0, 1) WITH NOWAIT;"
$initSql += "GO"
$initSql += ""
$initSql += "CREATE LOGIN fishspin_user WITH PASSWORD = '$($envHash.DB_PASSWORD)';"
$initSql += "PRINT 'Login fishspin_user created';"
$initSql += "GO"
$initSql += ""
$initSql += "CREATE DATABASE $($envHash.DATABASE_NAME);"
$initSql += "PRINT 'Database $($envHash.DATABASE_NAME) created';"
$initSql += "GO"
$initSql += ""
$initSql += "USE $($envHash.DATABASE_NAME);"
$initSql += "GO"
$initSql += ""
$initSql += "CREATE USER fishspin_user FOR LOGIN fishspin_user;"
$initSql += "PRINT 'User fishspin_user created in database';"
$initSql += "GO"
$initSql += ""
$initSql += "ALTER ROLE db_owner ADD MEMBER fishspin_user;"
$initSql += "PRINT 'Permissions granted to fishspin_user';"
$initSql += "GO"

$initSql | Set-Content "init.sql"