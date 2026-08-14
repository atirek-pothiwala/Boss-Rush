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

### WebGL / GitHub Pages

The game deploys automatically to GitHub Pages when changes are pushed to `main`.

**Live URL:** https://atirek-pothiwala.github.io/Boss-Rush/

#### One-time setup (repository owner)

Unity **Personal** uses Named User Licensing in Hub. There is **no serial in your Unity account**, and [license.unity3d.com](https://license.unity3d.com/manual) is for **Pro** licenses only (it will ask for a serial key). That is expected — do not use the manual website for Personal.

GameCI needs a one-time **activation serial** extracted from a `Unity_lic.ulf` file that Unity Hub writes on your Mac.

1. **Force Hub to create `Unity_lic.ulf` on your Mac**

   Quit Unity Hub, then in **Terminal**:

   ```bash
   sudo mkdir -p "/Library/Application Support/Unity"
   sudo chmod 777 "/Library/Application Support/Unity"
   ```

   Open Unity Hub → **Settings → Licenses → Add → Get a free personal license** (click **Add** even if Personal already appears).

   Open the project once in the Unity Editor if the file still does not appear.

   Check for the file:

   ```bash
   find /Library ~/Library -name "Unity_lic.ulf" 2>/dev/null
   ```

   `~/Library/Unity/licenses/UnityEntitlementLicense.xml` confirms Hub login worked, but **GameCI cannot use that XML file**.

2. **Extract your activation serial**

   ```bash
   chmod +x scripts/extract-unity-serial.sh
   ./scripts/extract-unity-serial.sh "/Library/Application Support/Unity/Unity_lic.ulf"
   ```

   Copy the output (format like `F-XXXX-XXXX-XXXX-XXXX-XXXX`).

3. **Add GitHub Actions secrets** (**Settings → Secrets and variables → Actions**):

   - `UNITY_EMAIL` — your Unity account email
   - `UNITY_PASSWORD` — your Unity account password  
     If you sign in with Google/GitHub, set a Unity password at https://id.unity.com first.
   - `UNITY_SERIAL` — paste the serial from step 2

4. Enable Pages: **Settings → Pages → Build and deployment → Source → GitHub Actions**
5. Run **Actions → Deploy WebGL to GitHub Pages → Run workflow** (branch `main`)

The workflow activates your license in CI, builds WebGL, returns the license, and publishes via `actions/deploy-pages`.

**Note:** Personal licenses have a concurrent activation limit. The workflow returns the license after each run so the next build can succeed.

#### Local WebGL build

Use the **WebGL** build profile under `Assets/Settings/Build Profiles/`.

```bash
# Cloud Agent / CI validation (optional UNITY_EMAIL + UNITY_PASSWORD)
./.cursor/scripts/validate.sh
```

## Cloud Agent environment

This repo includes a Cursor Cloud Agent environment:

- Install: `.cursor/scripts/cloud-agent-install.sh`
- Config: `.cursor/environment.json`

Set `UNITY_EMAIL`, `UNITY_PASSWORD`, and `UNITY_SERIAL` as environment secrets to enable optional batch-mode compile validation.

## Tests

Edit-mode tests live in `Assets/Tests/` and cover save/load logic and game constants. Run them from the Unity Test Runner window.

## License

See repository license for asset and code terms.
