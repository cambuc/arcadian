# Arcadian

**Arcadian** is a survival game demo built in Unity with C#, aiming to capture wilderness survival as realistically as possible.

## Features

- **Terrain Creation Tools**
  - Converts heightmap into a usable in-game topographic map
  - Procedural terrain generation utilities

- **Procedural Environment Placement**
  - Randomized object placement based on:
    - Biomes / zones
    - Terrain height
    - environmental rules

- **Dynamic Seasons**
  - Terrain and flora change based on seasonal cycles

- **Realistic Sun Movement**
  - Uses real DateTime
  - Adjusts brightness and sky position based on time of year
  - Sun position calculated with **sin/cos wave functions**

- **LOD Tree System**
  - Performance-optimized vegetation rendering

- **Fauna Navigation**
  - Animals roam around the world to preset locations
  - Can be spooked by the player through loud noise or getting too close

- **Crafting System**
  - Modular crafting framework for tools and survival items

- **Physical Inventory**
  - Tile-based inventory system with spatial item placement

- **Advanced Interaction System**
  - Objects can expose multiple interaction types
  - HUD menu allows players to scroll through available actions
