MonoFactory - Project README
Description

MonoFactory is a 2D action-adventure game with factory automation elements developed using C#, .NET 8, and the MonoGame framework. The project demonstrates the application of SOLID principles and various design patterns (Strategy, State, Factory, Command) in a game development context.
Prerequisites

    .NET 8.0 SDK

    Visual Studio 2022 (recommended) or VS Code

How to Run

    Navigate to the project directory: cd projectcopy

    Restore dependencies: dotnet restore

    Run the application: dotnet run

Controls

    Movement: ZQSD or Arrow Keys

    Jump: Space

    Attack: Left Mouse Button

    Interact: E (Talk to machines, etc.)

    Quick Transfer: R (Move items inventory <-> machine)

    Menu/Confirm: Enter

    Debug Mode: F3 (Toggle hitboxes)

    Exit: Escape

Gameplay Features

    Combat: Melee combat system with health management and enemy AI (Chasers, Patrollers, Turrets).

    Crafting: Gather resources (Iron Ore, Sticks) and use machines (Furnace, Crafter) to build weapons.

    Inventory: Manage items and equipment.

    Progression: Defeat enemies to spawn portals and advance through 7 levels, culminating in a boss fight.

Architecture & Design Patterns

This project fulfills software engineering course requirements by implementing the following:

    SOLID Principles: Extensive use of Single Responsibility (Components), Open/Closed (EntityFactory), and Dependency Inversion (InputReader).

    Strategy Pattern: Handles Enemy AI behavior (Chase, Patrol, Stationary).

    State Pattern: Manages Machine logic (Waiting, Processing, Finished).

    Factory Pattern: Centralizes entity creation logic.

    Command Pattern: Decouples input from game actions (Attack, Interact).
