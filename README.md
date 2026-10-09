# City Treasury

A Cities: Skylines II code mod that manages in game money, three ways:

1. **Safety net** (on by default): pick a treasury floor. If your balance drops below it, the mod tops it back up automatically. You never go broke mid build again.
2. **Cash control**: from the options page, add a chosen amount with one switch, or set the treasury to an exact balance.
3. **Budget autopilot** (off by default): watches the monthly trend. Two deficit months in a row nudges taxes up a small step; three surplus months nudges them down. Steps are 1 to 3 percent with hard caps, so it cannot spiral. Note: the tax binding needs a one time confirmation pass against your installed game version (see CLAUDE.md stage 3). Until then the autopilot logs what it would do and changes nothing.

Everything is controlled from **Options > City Treasury** in game. No custom UI panel, nothing to learn.

## One time setup (Shadow VM)

You only do this once. It takes about 15 minutes, most of it waiting on installers.

1. **Install the Modding Toolchain from inside the game.** Launch Cities Skylines 2, go to Options > Modding, and install every item until all checkmarks are green. This installs the .NET SDK and the project tools the mod builds with.
2. **Install Git** if the VM does not have it: `winget install Git.Git` in PowerShell, then close and reopen PowerShell.
3. **Get this repo onto the VM.** Either download it from GitHub as a zip and unzip it, or clone it (next step does that for you if you run install.ps1 from a copy of the repo).
4. **Run the installer.** Right click `install.ps1` > Run with PowerShell. It checks the toolchain, clones or updates the repo into `Documents\CS2Mods`, builds the mod, and the toolchain deploys it into the game's Mods folder automatically.
5. **Play.** Launch the game, enable City Treasury under Mods, load a city, open Options > City Treasury, set your floor.

If the build throws errors about game API names (possible after big game patches, and expected on the very first build for two known spots), that is what `CLAUDE.md` in this repo is for: open the repo in Claude Code on the VM and tell it to follow the brief. It will fix the bindings against your actual game files.

## Updating

Two kinds of updates, both simple:

- **Mod updates** (new features or fixes from GitHub): close the game, double click a desktop shortcut to `update.ps1` (right click the file > Send to > Desktop once to make it). It pulls the latest code and rebuilds. Relaunch.
- **After a Cities Skylines 2 game update**: game patches occasionally change internal APIs and any code mod can break. Routine: run `update.ps1` (a rebuild against the new game files fixes most cases), launch the game, and check the mod loads. If it does not, open the repo in Claude Code on the VM and have it follow CLAUDE.md, or send the Player.log to Muse in chat and the fix will be pushed to GitHub for you to pull.

Version is in `CityTreasury.csproj` and the in game mod list. Changes are logged in `CHANGELOG.md`.

## Publishing

This mod is built for local use on Ross's Shadow VM. It is **not** published to Paradox Mods. Publishing is a separate, explicit decision: the publish metadata is stubbed in `Properties/PublishConfiguration.xml` if that day ever comes.

## Layout

- `Mod.cs` - entry point, registers settings and the treasury system
- `Setting.cs` - the whole options page (plus English labels)
- `Systems/TreasurySystem.cs` - safety net, cash control, autopilot trend logic
- `Systems/TaxAdapter.cs` - the one isolated file that touches tax rates
- `install.ps1` / `update.ps1` - Shadow VM lifecycle scripts
- `CLAUDE.md` - build and repair brief for Claude Code on the VM
