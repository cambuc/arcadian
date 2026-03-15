# Arcadian

**Arcadian** is a survival game demo built in **Unity with C#**, aiming to capture wilderness survival as realistically as possible.  
This repository serves as a **technical portfolio project** demonstrating gameplay systems, procedural environment tools, and clean Unity architecture.

---

## Overview

Arcadian focuses on realistic survival mechanics and dynamic environments.  
The project emphasizes **system design, procedural generation, and gameplay architecture** rather than a finished commercial game.

---

## Features

- **Crafting System**
  - Modular crafting framework for tools and survival items

- **Physical Inventory**
  - Tile-based inventory system with spatial item placement

- **Advanced Interaction System**
  - Objects can expose multiple interaction types
  - HUD menu allows players to scroll through available actions

- **Terrain Creation Tools**
  - Converts heightmap into usable in-game **topographic map**
  - Procedural terrain generation utilities

- **Procedural Environment Placement**
  - Randomized object placement based on:
    - Biomes / zones
    - Terrain height
    - environmental rules

- **Dynamic Seasons**
  - Terrain and flora change based on seasonal cycles

- **Realistic Sun Movement**
  - Uses real **DateTime**
  - Adjusts brightness and sky position based on time of year
  - Sun position calculated with **sin/cos wave functions**

- **LOD Tree System**
  - Performance-optimized vegetation rendering

- **Fauna Navigation**
  - Animals roam around the world to preset locations
  - Can be spooked by the player through loud noise or getting too close

---

## Tech Stack

- **Engine:** Unity  
- **Language:** C#  
- **Focus:** Gameplay systems, procedural tools, environment simulation

---

## Purpose

This project exists to **demonstrate Unity development skills to potential employers**, including:

- Gameplay system architecture
- Procedural world tools
- Performance considerations
- Clean, maintainable C# code

This demo is provided for evaluation purposes only. All code, assets, and design are © Cameron Buchanan. Redistribution or reuse without permission is prohibited.