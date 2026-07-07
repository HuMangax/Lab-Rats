# Lab Rats

A retro-styled 2D puzzle platformer built with Unity. You play as a lab rat escaping through a series of laboratory test chambers — press buttons, grab keys, push boxes, and dodge spikes to unlock the exit door of each level.

## Gameplay

- **Move** with A/D or the arrow keys, **jump** with Space (standard Unity Input System bindings, gamepad supported).
- Each level is a self-contained puzzle: activate every switch connected to a door to open it, and reach the exit door to advance.
- Touching spikes sends you back to the level's start position.
- A level timer tracks how fast you clear each chamber.

### Objects

| Object | Behavior |
| --- | --- |
| Button | Activates while something (the rat or a box) is pressing it |
| Key | Follows the player once collected; unlocks its matching door on contact |
| Door | Opens only when **all** of its linked activators are active |
| Box | Pushable weight for holding buttons down |
| Jump Pad | Launches the player upward |
| Spike | Hazard — respawns the player at the start |
| Exit Door | Finishes the level and moves you to the next one |

## Project structure

- [Assets/Scripts](Assets/Scripts) — all gameplay code. The interaction system is built on two small base classes: `Activator` (buttons, keys — anything that emits an on/off state) and `Unlockable` (doors, toggleable tiles — anything that unlocks when all of its activators are active).
- [Assets/Scenes](Assets/Scenes) — `MainMenu`, `LevelSelect`, `Level_1`–`Level_3`, and `Congratulations` (end credits).
- [Assets/Prefab](Assets/Prefab) — reusable level pieces (Player, Box, Button, Door, Key, Jump Pad, Timer, level Grid).
- [Assets/Sprites](Assets/Sprites) — 8-bit / PICO-8-style pixel art.
- Levels are laid out with Unity Tilemaps ([Assets/Tile Pallete](Assets/Tile%20Pallete)).

Menus (main menu, level select, pause) build their UI in code at runtime rather than in the scene.

## Requirements

- **Unity 6000.4.0f1** (Unity 6) with the 2D and Universal Render Pipeline packages (installed automatically from `Packages/manifest.json`).

## Getting started

1. Clone the repository:
   ```sh
   git clone https://github.com/HuMangax/Lab-Rats.git
   ```
2. Open the project folder in Unity Hub with Unity 6000.4.0f1 (or a newer Unity 6 release).
3. Open `Assets/Scenes/MainMenu.unity` and press Play.
