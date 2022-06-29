rmdir -LiteralPath Migrations -Force -Recurse
dotnet build
dotnet ef migrations add InitialCreate --context $specifiedsolutionname$Context
dotnet build
dotnet ef database update --context $specifiedsolutionname$Context
