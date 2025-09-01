# 🐶 DogMaze Labyrinth Game

**Unity version:** 2022.3.44f1 (LTS)

---

## How to Run
1. Clone the repo: https://github.com/Jambolul/dogmaze.git  
2. Open the project in **Unity 2022.3.44f1 (LTS)**.  
3. Open the **MainMenu** scene.  
4. Press **Play** in the Unity Editor or build the project into an executable.  

---

## Controls
- Move the dog with your **mouse cursor**.  
- The dog smoothly follows a custom-made tennis ball cursor.  
- Avoid walls → touching one causes **Game Over**.  
- Reach the bone → you **Win** and hear the goal sound.  

---

## Features Implemented
- Procedurally generated labyrinths with 2D colliders (BoxCollider2D for walls, CircleCollider2D for dog).  
- Main Menu with **Start**, **Settings**, and **Quit**.  
- Settings menu with a **Master Volume slider**.  
- **Win/Lose overlays** with Retry and Main Menu buttons.  
- Dog follows the mouse.  
- Custom pixel-art tennis ball cursor.  
- Ambience loop + goal SFX.  

---

## Known Issues / Future Work
- Could add a settings option to change maze size/difficulty.  
- Menu and UI could be polished to look nicer (currently minimal).
- Could add features such as best time list of some sort.
