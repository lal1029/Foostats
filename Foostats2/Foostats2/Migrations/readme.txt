Instructions:
To create a migration in Package Manager console: Add-Migration <Migration-Name>
To create the script to run the migration: Update-Database -Script -SourceMigration:$<PreviousMigration>
Add the script to the SqlScripts folder