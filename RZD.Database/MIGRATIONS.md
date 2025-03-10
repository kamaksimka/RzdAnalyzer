**Prerequisites**

```dotnet tool install --global dotnet-ef```

**In Solution Folder**

Api project must be stopped (not running) before doing migrations

|Operation|Command|
|---|---|
|Create Migration|```dotnet ef migrations --project RZD.Database --startup-project RZD.Scheduler add <NAME>```|
|Preview Update SQL|```dotnet ef migrations --project RZD.Database --startup-project RZD.Scheduler script```|
|Update Database|```dotnet ef database --project RZD.Database --startup-project RZD.Scheduler update```|
