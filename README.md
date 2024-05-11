# Crypt Crawlers

I'm developing a 3D dungeon crawler multiplayer game.

## Dungeon Generation

Dungeon generation is completed. Each level features a set number of mandatory rooms, a boss room, and optional rooms that can be locked. The program uses a delaunay triangulation algorithm and an A\* search algorithm to pathfind hallways.

- Red: Optional or mandatory rooms
- Black: Boss room
- Purple: Main hallways that never lead to locked doors
- Blue: Optional hallways that can lead to locked doors
- Green: Locked door
- Orange: Unlocked door

![Example Dungeon:](https://github.com/nicksimache/dungeon-crawler/blob/main/LevelGenerator/dungeongenerateexample.png)

## Enemy and Player Mechanics

Currently working on enemy and player mechanics
