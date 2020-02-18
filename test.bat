@echo off

dotnet build
dotnet test .\Hyperspool.Tests\Hyperspool.Tests.csproj

PAUSE