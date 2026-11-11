# CosmoTweest — Fleet Command

Run `CosmoTweest.exe` in the parent **CosmoTweest FINAL** folder. A normal launch updates the existing desktop shortcut with the rocket icon. This is the permanent delivery folder for future updates. Scores, unlocked sectors, sound settings, and mouse-flight preferences migrate from the existing local save.

## Branching ship upgrades

Gold upgrade cores still become available at 1,500, 5,000, and 11,000 points. Collecting one pauses the action and opens a three-hull choice. Click a card, use Left/Right and Enter, or press 1–3. You can switch paths at the next core, or keep improving the same path through ranks I, II, and III.

| Path | Benefit | Tradeoff |
| --- | --- | --- |
| Interceptor | 20% faster movement, 15% shorter cannon interval, 2.2-second boost cooldown | No missiles or regenerating shield |
| Missile Gunship | Homing missiles with splash damage, in addition to primary cannons; twin missiles at rank III | 10% slower movement and slower primary cannon cadence |
| Shield Tank | Regenerating shield: 55 / 70 / 85 capacity by rank; restores 9 points/second after four seconds without a hit | 20% slower movement |

Normal multi-cannon volleys use one magazine round. Gunship missiles launch alongside eligible firing volleys on a separate cooldown and require player ammunition to be firing. Drones use their own free shots. Collecting an upgrade retains the existing supply/protection reward. The game stays paused until a hull is chosen, then resumes with brief protection. Mouse steering is adjusted for each hull's movement speed without overshooting the pointer.

Ship rank, branch, and equipped drones survive sector transitions and retrying the current sector. Retrying restores that hull's shield. A new campaign starts with the Scout and no drones.

## Sector mini-bosses

Each of the first four sectors introduces a mini-boss about 22 seconds into the approach. Ordinary wave spawning pauses while that mini-boss is alive; the carrier can still deploy escorts. The sector cannot finish while the encounter remains alive. Sector five retains the final dreadnought battle.

- **Raider Ace:** aimed three-shot attacks and lateral movement.
- **Belt Harvester:** radial fire and falling asteroids.
- **Brood Carrier:** spread attacks and launched escort fighters.
- **Ion Warden:** rotating bullet rings plus a fast aimed shot.

Two glowing side pods protect each core. While a pod remains, the core takes only 30% damage. Each pod has its own health indicator; destroying both exposes the core to full damage. Missiles and bombs can damage these parts. Each mini-boss awards score, ammunition, and a cyan defence-drone core.

## Defence drones

Fly into a cyan drone core to equip a companion, up to two. Drones orbit the rocket, fire automatically while you fire (including auto-fire), and periodically intercept nearby hostile projectiles. A single drone has a shared 1.4-second interception cooldown; two reduce that to one second. Interception continues when you are not firing. An additional core with a full drone bay converts into 60 rounds and 20 hull, within normal caps.

The HUD shows drone count, shield capacity, current ship path, difficulty, and encounter health. Cyan drone pickup cores remain collectible across sector transitions.

## Difficulty and auto-fire

Open **Settings → Flight & Difficulty**. Difficulty changes apply to the next new campaign or sector selection, including Play Again after victory. Restarting the active sector retains its original difficulty. Mouse/keyboard controls and auto-fire can be changed during a paused run.

| Setting | Relaxed | Standard | Expert |
| --- | --- | --- | --- |
| Starting ammo / bombs | 240 / 4 | 180 / 3 | 160 / 2 |
| Enemy/projectile speed multiplier | 0.78× | 1× | 1.22× |
| Incoming damage multiplier | 0.65× | 1× | 1.30× |
| Ordinary spawn interval multiplier | 1.25× | 1× | 0.8× |
| Supply interval | 4.2 seconds | 5.5 seconds | 6.5 seconds |
| Mini/final boss HP multiplier | 0.85× | 1× | 1.18× |

Auto-fire is optional and off by default. When enabled, it fires during active play without holding left mouse, consuming ammunition normally. It freezes during pause, menus, transitions, and ship selection. Bombs and boost retain their usual controls. M still toggles sound, with separate master/music/effects sliders.

Personal bests are stored separately for the three modes. An existing pre-difficulty high score is preserved as the Standard record, and existing sector unlocks remain intact.

## Verification — 11 November 2026

- **97 gameplay/settings assertions passed**, covering prior controls/audio/gameplay and branch choice validation/freezing, path tradeoffs, homing and splash damage, shield absorption/regeneration, miniature boss armor/pods/rewards, drone firing/interception/cooldowns/cap, retry loadout, difficulty settings/resources/damage/speed, separate score records, and legacy save migration.
- **All nine path/difficulty campaign combinations reached victory** in deterministic simulation. Every run defeated all four mini-bosses, reached rank III on its selected path, and equipped two drones. The QA pilots repair low hull as needed; their repair counts are recorded. This verifies progression and stability rather than unaided human difficulty balance.
- **48 live application assertions passed**, including the paused hull-choice screen, numeric-key path selection, live mini-boss, drone rewards/fire/interception, persisted flight preferences, automatic firing, per-run difficulty retention, and applying the next-run difficulty from victory.
- The actual rendered executable also completed a campaign through victory: 284.4 simulated seconds, four mini-bosses, Shield Tank III, two drones, 31 interceptions, and 40 launched missiles. The QA pilot repairs low hull. No crash occurred during these completed tests.
- The 32-second native-window sector-four benchmark at 1100×800 measured **50.0 FPS**, **7.83 ms** mean drawing, and **11.62 ms** 95th-percentile drawing on the available Windows 10 Pro 22H2 / Intel i5-2400 / GTX 1660 machine. This replaces the prior edition's FPS figure for this release. Display scanout and input latency are not measured.
- Branch cards, mini-bosses, flight settings, and the fleet artwork preview were rendered and visually inspected. Input tests invoke application event handlers; a physical mouse/keyboard playthrough and human audio evaluation were not performed. Human difficulty balance, clean-PC installation, and signing remain unverified; the executables are unsigned.

Raw verification files are in `tests\Verification\fleet-*.txt`. The installer payload, icon shortcut, and uninstall paths are checked in a project-local test folder; a normal launch updates the actual desktop shortcut.

## Source and build

Run `build.ps1` in this Source folder to update the parent final folder. The Windows .NET Framework C# compiler produces the single-file game and EXE installer without external packages. Windows 10/11's normal .NET Framework 4.x component is required to play; development tools are not.

The added systems are in `src\FleetSystems.cs`, `src\FleetArt.cs`, `src\FleetMenus.cs`, `src\FleetTests.cs`, and `src\FleetQa.cs`. Original art/audio licensing and credits remain unchanged. `--selftest` runs the nine-campaign simulation matrix and writes raw results; `--qa` exercises application states; `--benchmark` measures the native window; `--autoplay --report <absolute-path>` runs a rendered campaign. Test modes use isolated settings rather than the player's save.
