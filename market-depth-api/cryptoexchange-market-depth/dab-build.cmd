@echo off
@echo This cmd file creates a Data API Builder configuration based on the chosen database objects.
@echo To run the cmd, create an .env file with the following contents:
@echo dab-connection-string=your connection string
@echo ** Make sure to exclude the .env file from source control **
@echo **
dotnet tool install -g Microsoft.DataApiBuilder
dab init -c dab-config.json --database-type mssql --connection-string "@env('dab-connection-string')" --host-mode Development
@echo Adding tables
dab add "Ask" --source "[dbo].[Asks]" --fields.include "Id,OrderBookSnapshotId,Price,Amount" --permissions "anonymous:*" 
dab add "Bid" --source "[dbo].[Bids]" --fields.include "Id,OrderBookSnapshotId,Price,Amount" --permissions "anonymous:*" 
dab add "Snapshot" --source "[dbo].[Snapshots]" --fields.include "Id,AcquiredAt,Timestamp,Microtimestamp" --permissions "anonymous:*" 
@echo Adding views and tables without primary key
@echo Adding relationships
dab update Ask --relationship Snapshot --target.entity Snapshot --cardinality one
dab update Snapshot --relationship Ask --target.entity Ask --cardinality many
dab update Bid --relationship Snapshot --target.entity Snapshot --cardinality one
dab update Snapshot --relationship Bid --target.entity Bid --cardinality many
@echo Adding stored procedures
@echo **
@echo ** run 'dab validate' to validate your configuration **
@echo ** run 'dab start' to start the development API host **
