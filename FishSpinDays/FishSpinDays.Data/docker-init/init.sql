-- Create login if not exists
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'fishspin_user')
BEGIN
 CREATE LOGIN fishspin_user WITH PASSWORD = 'FishSpinPassword123!';
 PRINT 'Login fishspin_user created';
END
GO

-- Create database if not exists
IF DB_ID('FishSpinDays') IS NULL
BEGIN
 CREATE DATABASE FishSpinDays;
 PRINT 'Database FishSpinDays created';
END
GO

USE FishSpinDays;
GO

-- Create user in database if not exists
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'fishspin_user')
BEGIN
 CREATE USER fishspin_user FOR LOGIN fishspin_user;
 PRINT 'User fishspin_user created in database';
END
GO

-- Grant permissions
ALTER ROLE db_owner ADD MEMBER fishspin_user;
PRINT 'Permissions granted to fishspin_user';
GO
