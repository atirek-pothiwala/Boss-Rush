# Boss Rush

A 2D boss-rush fighting game built with **Unity 6** (`6000.4.3f1`) by Knight Wing. Fight three bosses in sequence — Minotaur, Werewolf, and Gorgon — as one of three heroes: Samurai, Shinobi, or Fighter.

## Features

- Hero selection with unique stats per character
- Three bosses with distinct attack patterns and per-boss battle music
- Enrage phase when bosses drop below 50% HP
- Save/resume progress (hero + current boss)
- Settings for music and SFX volume
- In-game controls reference

## Requirements

- [Unity 6000.4.3f1](https://unity.com/releases/editor/whats-new/6000.4.3f1)
- [Git LFS](https://git-lfs.com/) — required for art, audio, and font assets

## Setup

```bash
git clone https://github.com/atirek-pothiwala/Boss-Rush.git
cd Boss-Rush
git lfs pull
```

Open the project in Unity Hub and load `Assets/Scenes/Main Menu.unity`.

## Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | WASD / Arrows | Left Stick |
| Run | Shift | RB |
| Jump | Space | A |
| Quick Attack | LMB | Y |
| Power Attack | RMB | X |
| Special Attack | E | B |
| Shield | Q | LB |
| Pause | Escape | Start |

## Heroes

| Hero | Style | Health | Speed | Damage |
|------|-------|--------|-------|--------|
| Samurai | Balanced | 100 | Normal | 1.0× |
| Shinobi | Fast, agile | 90 | Fast | 0.85× |
| Fighter | Tank, heavy hits | 120 | Slow | 1.2× |

## Boss order

1. **Minotaur** — charge attacks and quick strikes
2. **Werewolf** — aerial jump attacks
3. **Gorgon** — ranged scream when enraged

## Building

### Standalone (Windows)

Use the **Windows** build profile under `Assets/Settings/Build Profiles/`.

### WebGL

Use the **WebGL** build profile. Output can be hosted on GitHub Pages or any static file host.

```bash
# Cloud Agent / CI validation (requires Unity license secrets)
./.cursor/scripts/validate.sh
```

## Cloud Agent environment

This repo includes a Cursor Cloud Agent environment:

- Install: `.cursor/scripts/cloud-agent-install.sh`
- Config: `.cursor/environment.json`

Set `UNITY_EMAIL` and `UNITY_PASSWORD` (or `UNITY_LICENSE`) as environment secrets for batch-mode compile validation.

## Tests

Edit-mode tests live in `Assets/Tests/` and cover save/load logic and game constants. Run them from the Unity Test Runner window.

## License

See repository license for asset and code terms.
