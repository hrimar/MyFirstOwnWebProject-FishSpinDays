RAISERROR('Init script started', 0, 1) WITH NOWAIT;
GO

CREATE LOGIN fishspin_user WITH PASSWORD = 'FishSpinPassword123!';
PRINT 'Login fishspin_user created';
GO

CREATE DATABASE FishSpinDays;
PRINT 'Database FishSpinDays created';
GO

USE FishSpinDays;
GO

CREATE USER fishspin_user FOR LOGIN fishspin_user;
PRINT 'User fishspin_user created in database';
GO

ALTER ROLE db_owner ADD MEMBER fishspin_user;
PRINT 'Permissions granted to fishspin_user';
GO