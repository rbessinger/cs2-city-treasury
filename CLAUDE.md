# CLAUDE.md - City Treasury build and repair brief

You are working on Ross's Cities: Skylines II code mod, City Treasury, on his Windows gaming VM. Ross directs and reviews; you do the hands on work. Work in stages, one git commit per stage, and keep him posted in plain language.

## Ground rules

- Idempotent pipeline: every script and build step must be safe to re-run and skip work already done.
- Commit after each stage below lands and builds. Never commit a build that fails.
- Never publish to Paradox Mods. Local Mods folder only.
- Keep game API uncertainty quarantined: the only files allowed to guess at internal game APIs are `Systems/TreasurySystem.cs` (treasury access) and `Systems/TaxAdapter.cs` (tax rates). Everything else uses public template API only.
- The mod must always be safe to enable: any feature whose binding is not yet confirmed must log and do nothing.

## Stage 1: Toolchain sanity

1. Confirm the Modding Toolchain is installed: the user env var `CSII_TOOLPATH` exists and `%CSII_TOOLPATH%\Mod.props` is present.
2. Confirm `dotnet --version` works in a fresh PowerShell.
3. `dotnet build -c Release` in the repo root. The toolchain post build step deploys to `%LOCALAPPDATA%\Low\Colossal Order\Cities Skylines II\Mods\CityTreasury`.
4. Launch the game with `--developerMode`, enable the mod, load a city, confirm "City Treasury" appears under Options and the log shows OnLoad. Player.log is at `%LOCALAPPDATA%\Low\Colossal Order\Cities Skylines II\Player.log`.

## Stage 2: Verify treasury bindings (expected first build fixes)

Confirm against the installed Game.dll (Visual Studio Object Browser, or decompile `Cities2_Data\Managed\Game.dll` with ILSpy):

1. `Game.City.PlayerMoney`: the balance field/property name (code assumes `money`). Fix TreasurySystem if it differs.
2. `Game.City.CitySystem`: how to get the city entity (code assumes `.City`). Fix if it differs.
3. `Game.Simulation.SimulationSystem.frameIndex` and the frames per in game day constant used in `CurrentGameMonth` (code assumes 262144). If the time model differs, fix the constant or switch to the game's date API. Autopilot only needs month boundaries, approximation is acceptable.
4. Rebuild, then in game test: flip "Add cash now" with a small amount and confirm the balance moves and the switch turns itself off. Test the safety net by setting a floor above the current balance.

## Stage 3: Bind the tax adapter

1. In Game.dll, find `Game.Economy.TaxSystem` and how residential, commercial, and industrial tax rates are stored and written (rate array and any setter or component write path).
2. Implement `TaxAdapter.AdjustTaxes` honoring the contract in that file: move the three main rates together by the step, clamp every rate into [0, 30] percent, never move more than the step in one call.
3. Set `s_Bound = true` only after an in game test: enable autopilot with step 1, force a deficit trend (or temporarily lower the trigger thresholds in a debug build), and confirm rates move exactly one step and respect the caps.
4. If TaxSystem turns out to be read only from mods, stop and report that to Ross instead of reaching for Harmony patches without his go ahead.

## Stage 4: After any game update (repeat as needed)

1. Rebuild against the updated game files (`dotnet build -c Release`).
2. If compile errors reference game APIs, re run the stage 2 and 3 verifications for the broken names only, fix, rebuild, commit with a message naming the game version.
3. Smoke test in game: mod loads, options page opens, add cash works.
4. Bump `Version` in CityTreasury.csproj and add a CHANGELOG.md entry.

## Definition of done (v1)

- Mod builds clean and deploys via the toolchain.
- Safety net and cash control verified working in game.
- TaxAdapter bound and verified, or clearly reported as blocked with the reason.
- install.ps1 and update.ps1 both re-run cleanly end to end.
