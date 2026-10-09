# City Treasury - Ross's Setup Steps

Do these in order. Steps 1 is from your phone or any browser. Steps 2 to 7 are inside the Shadow PC. Total hands on time is about 20 minutes plus installer waits.

## Step 1: Create the GitHub repo (phone or browser, 30 seconds)

1. Go to github.com/new
2. Name: cs2-city-treasury
3. Set it to Private
4. Do NOT check "Add a README" (the repo must be empty)
5. Create it, then tell Muse "repo is ready" in chat. Muse pushes all the mod files into it.

## Step 2: Install the Modding Toolchain (in Shadow)

1. Launch Cities Skylines 2
2. Options > Modding
3. Install every item until all checkmarks are green
4. Close the game

## Step 3: Install Git and Claude Code (in Shadow, PowerShell)

Open PowerShell and run:

    winget install Git.Git

Then install Claude Code for Windows from claude.com (or `winget install Anthropic.ClaudeCode` if listed). Close and reopen PowerShell when both finish.

## Step 4: Get the mod onto the Shadow

In PowerShell:

    cd $env:USERPROFILE\Documents
    mkdir CS2Mods -Force
    cd CS2Mods
    git clone https://github.com/rbessinger/cs2-city-treasury.git
    cd cs2-city-treasury

## Step 5: First build, with Claude Code driving

1. In the cs2-city-treasury folder, start Claude Code
2. Tell it exactly: "Follow CLAUDE.md. Start at stage 1."
3. It verifies the toolchain, runs the first build, fixes the two expected API binding spots against your game files, and commits per stage
4. Stay nearby for any permission prompts; the game launch test needs you to click into the game

## Step 6: Turn it on and test (in game)

1. Launch Cities Skylines 2, enable City Treasury in the Mods menu
2. Load your city
3. Options > City Treasury: confirm your safety net floor (default $100,000)
4. Test cash control: set Cash Amount to something small like $10,000, flip "Add cash now", watch the balance move and the switch turn itself off
5. If anything fails to load, grab the log at %LOCALAPPDATA%\Low\Colossal Order\Cities Skylines II\Player.log and send it to Muse in chat

## Step 7: Make updates one click (one time, in Shadow)

1. In the cs2-city-treasury folder, right click update.ps1 > Send to > Desktop (create shortcut)
2. From then on: close the game, double click the shortcut, relaunch. That is every mod update and the first move after every game patch
