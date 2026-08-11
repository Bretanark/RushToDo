/*
    Post-deployment script for RushTodo.Database deployments.

    Keep this script idempotent. It runs after the DACPAC model changes are applied.
*/

:r ..\Scripts\Seed.sql

PRINT N'RushTodo.Database post-deployment script completed.';
GO
