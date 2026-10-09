# City Treasury - one time install (run inside the Shadow VM, in PowerShell)
# What it does: clones the repo if needed, then builds the mod.
# The CS2 Modding Toolchain's post build step deploys the built mod into
# your local Mods folder automatically. Safe to re-run: it skips work that
# is already done (idempotent).

$ErrorActionPreference = "Stop"
$RepoUrl = "https://github.com/rbessinger/cs2-city-treasury.git"
$Root = Join-Path $env:USERPROFILE "Documents\CS2Mods"
$Repo = Join-Path $Root "cs2-city-treasury"

Write-Host "== City Treasury install ==" -ForegroundColor Cyan

# 1. Toolchain check: CSII_TOOLPATH is set by the game's Modding Toolchain
#    installer (in game: Options > Modding, install everything to green).
if (-not [Environment]::GetEnvironmentVariable("CSII_TOOLPATH", "User")) {
    Write-Host ""
    Write-Host "The CS2 Modding Toolchain is not installed yet." -ForegroundColor Yellow
    Write-Host "Open Cities Skylines 2, go to Options > Modding, and install"
    Write-Host "every item until all checkmarks are green. Then run this again."
    exit 1
}

# 2. dotnet check (the toolchain installs a .NET SDK; verify it is on PATH)
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "dotnet is not on PATH yet. Restart this PowerShell window" -ForegroundColor Yellow
    Write-Host "(toolchain installs sometimes need a fresh shell), then re-run."
    exit 1
}

# 3. Clone or update the repo
New-Item -ItemType Directory -Force -Path $Root | Out-Null
if (Test-Path (Join-Path $Repo ".git")) {
    Write-Host "Repo already present, pulling latest..."
    git -C $Repo pull --ff-only
} else {
    Write-Host "Cloning the mod repo..."
    git clone $RepoUrl $Repo
}

# 4. Build (post build deploys into the game's Mods folder)
Write-Host "Building City Treasury (Release)..."
dotnet build (Join-Path $Repo "CityTreasury.csproj") -c Release

Write-Host ""
Write-Host "Done. Launch Cities Skylines 2, enable City Treasury in the" -ForegroundColor Green
Write-Host "Mods menu, load your city, and open Options > City Treasury."
