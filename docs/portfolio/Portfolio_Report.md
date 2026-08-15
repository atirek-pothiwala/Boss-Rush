# Boss Rush — Student Portfolio Report

**Track:** Game Development and Simulation  
**Project Type:** Level-based 2D Boss-Rush Action Game  
**Engine:** Unity 6 (6000.4.3f1)  
**Platform:** WebGL (GitHub Pages), Standalone  
**Live Demo:** https://atirek-pothiwala.github.io/Boss-Rush/  
**Repository:** https://github.com/atirek-pothiwala/Boss-Rush  

---

## Project Overview

**Boss Rush** is a 2D side-view fighting game where the player selects one of three heroes and fights through a sequence of three bosses: **Minotaur**, **Werewolf**, and **Gorgon**. Each boss has unique attack patterns, animations, and battle music. The player must manage health and stamina, block with a shield, and exploit openings after boss attacks to win.

The full run takes approximately **8–12 minutes** for a first-time player (three boss encounters with save/resume support). The project was built to demonstrate scalable Unity architecture, data-driven combat design, animator-driven boss AI, and a polished WebGL deployment pipeline.

---

## Features

### Core gameplay
- **Three playable heroes** with distinct stats (Samurai, Shinobi, Fighter)
- **Three boss encounters** with unique behaviours and enrage phases (below 50% HP)
- **Health and stamina systems** for both player and boss
- **Combat** — quick attack, power attack, special attack, shield, jump, run
- **Boss AI** — approach, attack selection by range/stamina, retreat, enrage, aerial attacks (Werewolf), scream (Gorgon)

### Progression and persistence
- **Boss-rush progression** — defeat a boss to unlock the next
- **Save / resume** — hero choice and current boss saved via `PlayerPrefs`
- **Continue** option on main menu when a saved run exists

### Menus and UI
- Main menu with Start, Settings, Controls, Exit
- Runtime-built hero selection, settings (music/SFX volume), and controls reference
- In-fight HUD — boss name, health/stamina bars for hero and boss
- Pause menu — Resume, Restart Level, Exit to Main Menu
- Victory / defeat overlays with next-boss or retry options

### Audio and presentation
- Per-boss battle themes and menu music via persistent `SoundManager`
- Dynamic camera that frames both fighters
- Landscape-locked layout with responsive WebGL shell (rotate prompt on mobile portrait)
- GitHub Actions CI — compile validation, EditMode tests, automated WebGL deploy

---

## Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | WASD / Arrow Keys | Left Stick |
| Run | Shift | RB (Right Bumper) |
| Jump | Space | A |
| Quick Attack | Left Mouse Button | Y |
| Power Attack | Right Mouse Button | X |
| Special Attack | E | B |
| Shield | Q | LB (Left Bumper) |
| Pause | Escape | Start |

---

## Techniques Used

### Architecture and code organisation
The project follows **single-responsibility** design with separated managers and systems under `Assets/Src/`:

| System | Responsibility |
|--------|------------------|
| `DashboardManager` | Main menu, hero selection, settings, controls UI |
| `LevelManager` | Spawns hero/boss, loads environment |
| `HealthManager` | Health/stamina values and regeneration |
| `UIManager` | Fight HUD, pause/victory/defeat overlays |
| `PauseManager` | Pause state, scene navigation |
| `SoundManager` | Music and SFX (DontDestroyOnLoad singleton) |
| `CameraManager` | Dynamic orthographic framing |
| `SceneTransition` | Safe deferred scene loading (WebGL) |
| `PlayerController` | Player movement, combat, animation |
| `BossController` | Boss AI, attacks, enrage, damage routines |
| `PlayerInputManager` | Unity Input System → static events |
| `Constants` / `GameSave` | Progression and persistence |

Assembly definition `BossRush.asmdef` keeps game code isolated from tests (`BossRush.Tests`).

### Data-driven combat
- **`PlayerAttackConfig`** and **`BossAttackConfig`** — serialisable attack data (damage, range, stamina cost, cooldown, sounds) configured per prefab
- **`HeroStats`** — applies hero-specific speed, damage, and health modifiers
- **`BossState` / `PlayerState` enums** — drive animator parameters for clean animation control

### State-driven animation
- Animator controllers per character/boss with hashed parameters (`State`, `Idle`, `Move`, `OnAction`)
- Boss attack selection from configured attack array with stamina and range checks
- Enrage phase increases boss speed and damage multiplier below 50% HP

### Input System
- **Unity Input System** (`Player Controls.inputactions`) with keyboard and gamepad bindings
- Event-based decoupling — `PlayerInputManager` fires static events; `PlayerController` subscribes
- `InputSystemBootstrap` keeps input processing active while `Time.timeScale = 0` (pause)

### UI techniques
- **Canvas Scaler** at 1280×720, match height for landscape
- Runtime UI construction in `DashboardManager` (hero cards, sliders, buttons)
- Anchors and layout groups on fight HUD

### Optimisation
- Cached references to boss/hero tags (lookup once, then reuse)
- Coroutine-based attack/damage routines instead of per-frame polling where possible
- `SceneTransition` defers scene loads to avoid WebGL delegate crashes
- Singleton cleanup in `OnDestroy` to prevent stale references during scene unload
- No `Debug.Log` in `Update` loops

### Testing and CI
- EditMode tests: `ConstantsTests`, `GameSaveTests`, `BossAttackAnimatorTests`
- GitHub Actions: Unity compile validation on every PR

### WebGL deployment
- Custom GitHub Pages template with landscape letterboxing and portrait rotate prompt
- `WebGLHelper` jslib for browser tab close on Exit

---

## Challenges

1. **Boss animator edge cases** — Minotaur had a stray `QuickAttack` state with no animator transition, causing idle freezes and phantom damage. Fixed by auditing prefab attack configs and adding failsafe recovery in `BossController`.

2. **WebGL scene transitions** — Exit/Restart caused `RuntimeError: null function` when loading scenes synchronously from UI callbacks. Solved with deferred `SceneTransition` loads and clearing static input events before unload.

3. **Pause menu on Escape** — `PressAndRelease` input interaction fired on both press and release, so the menu only appeared while Escape was held. Fixed by removing that interaction and using `started` for toggle.

4. **Mobile / WebGL layout** — Portrait mode and oversized UI text on phones. Addressed with landscape lock, canvas scaler tuning, and template-level rotate prompt.

5. **Singleton lifecycle** — `UIManager.Update` and `HealthManager` coroutines accessed destroyed managers during scene unload. Added null guards and proper `Instance` cleanup.

---

## Future Improvements

- Add a fourth boss or optional hard mode with remixed attack patterns
- Object pooling for VFX and hit effects if particle count grows
- Boss intro cutscenes and telegraph indicators for heavy attacks
- Achievements / leaderboard for fastest clear time per hero
- Localisation for menu text
- Full gamepad-focused UI navigation on pause menus
- Additional EditMode/playmode tests for combat damage and AI decision logic
- Recorded gameplay trailer embedded in the portfolio submission

---

## Screenshots

### Main Menu
![Main menu — title screen with Start Game, Settings, Controls, and Exit](images/main_menu.png)

### Hero Selection
![Hero selection — choose Samurai, Shinobi, or Fighter](images/hero_selection.png)

### Combat — Minotaur Boss Fight
![Combat against Minotaur — hero and boss health bars, arena environment](images/combat_minotaur.png)

### Pause Menu
![Pause menu — Resume, Restart Level, Exit to Main Menu](images/pause_menu.png)

### Defeat Screen
![Defeat screen — Samurai defeated by Minotaur with retry options](images/boss_defeated.png)

---

## Gameplay Video

| Item | Details |
|------|---------|
| **Live playable build** | https://atirek-pothiwala.github.io/Boss-Rush/ |
| **Suggested recording** | 5–8 minutes: menu → hero pick → boss fights → pause/resume |
| **Submission** | Record with OBS (or similar) while playing the WebGL build, then upload to YouTube/Google Drive and paste the link here |

**Recording checklist for submission:**
1. Main menu and hero selection
2. At least one full boss fight (Minotaur recommended for demo)
3. Pause menu (Escape) and Resume
4. Victory or defeat screen
5. Optional: Settings volume sliders and Continue saved run

*Replace this section with your video URL before final submission, e.g. `https://youtu.be/your-video-id`*

---

## Project Structure (Unity)

```
Assets/
├── Src/
│   ├── Boss/           # BossController, BossAttackConfig, BossState
│   ├── Player/         # PlayerController, input, attack configs
│   ├── Managers/       # Level, UI, Health, Pause, Sound, Camera, Dashboard
│   ├── Utils/          # Constants, GameSave, SceneTransition, HeroStats
│   └── Effects/        # BreathingEffect
├── Prefabs/Characters/ # Hero and boss prefabs
├── Scenes/             # Main Menu.unity, Fight Level.unity
├── Animations/         # Per-character animator controllers
├── Tests/              # EditMode unit tests
├── WebGLTemplates/     # GitHub Pages deploy template
└── Plugins/WebGL/      # Browser close-tab jslib
```

---

## Summary

Boss Rush meets the portfolio scope requirements for a level/enemy-based game: **three polished boss encounters**, **three enemy/character types**, **5–15 minutes** of gameplay, separated systems, animator state machines, data-driven attacks, responsive UI, optimisation awareness, and automated WebGL deployment. The documentation above follows the mandatory Section 1 format from the portfolio guidelines.

---

*Report generated for the Boss Rush Unity project. Screenshots captured from the live WebGL build (August 2026).*
