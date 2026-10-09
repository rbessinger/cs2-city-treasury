# City Treasury - update (run inside the Shadow VM, in PowerShell)
# Pulls the newest code from GitHub and rebuilds. The toolchain post build
# step redeploys the mod into the game's Mods folder. Close the game first
# if it is running, so the mod DLL is not locked.
# Make a desktop shortcut to this file and updating is a double click.

$ErrorActionPreference = "Stop"
$Repo = Join-Path $env:USERPROFILE "Documents\CS2Mods\cs2-city-treasury"

if (-not (Test-Path (Join-Path $Repo ".git"))) {
    Write-Host "Repo not found at $Repo - run install.ps1 first." -ForegroundColor Yellow
    exit 1
}

Write-Host "== City Treasury update ==" -ForegroundColor Cyan
git -C $Repo pull --ff-only
dotnet build (Join-Path $Repo "CityTreasury.csproj") -c Release
Write-Host ""
Write-Host "Updated. Relaunch the game and the new version is live." -ForegroundColor Green
