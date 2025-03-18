# HappyHour Case Study - Archero-Inspired Game Mechanics

## Overview
This project is a case study inspired by the game mechanics of **Archero**. The goal was to develop a scalable and optimized game architecture suitable for mobile platforms. Below, I outline the core mechanics, design patterns, and optimizations implemented in this project.

---

## Core Mechanics

### 1. **Control Mechanism**
The character is controlled using a **fixed joystick** on the screen. The joystick provides "Horizontal" and "Vertical" inputs, which are processed by the **Character Controller** to move the character. When the joystick is released, the character stops moving and starts attacking the nearest enemy within range.

---

### 2. **Combat Mechanics**
When the character stops moving, it automatically targets the nearest enemy within its attack range. The attack is physics-based, simulating a realistic arrow shot.

---

### 3. **Enemies**
As requested in the documentation, enemies are stationary, can only take damage, and never move. To make the gameplay visually appealing and engaging, I designed enemies as **explosive barrels filled with gunpowder**. Each enemy has a health bar displayed above it. There are always 5 enemies on the map, and when they are defeated, they respawn at random locations.

---

### 4. **Skill System**
All skills requested in the documentation have been implemented. These skills can be activated or deactivated with a single click via the UI. The skill system is designed to be **flexible and scalable**, allowing new skills to be added and activated easily.

---

## Design Patterns

### 1. **Dependency Injection Pattern**
The project uses the **Dependency Injection (DI)** pattern for dependency management. This choice was intentional, as the target platform is mobile, where device performance varies widely. DI ensures that the game runs smoothly on most devices by optimizing **RAM** and **CPU usage**. 

- **CPU Optimization:** Mathematical and physical calculations are performed at fixed intervals rather than every frame.
- **RAM Optimization:** The Unity Addressables system is used to load and unload assets dynamically, preventing memory leaks and crashes. DI ensures no hard references are created, making it easier to manage memory.

---

### 2. **Object Pool Pattern**
All reusable objects in the project (e.g., enemies, arrows, explosion effects) are cached using the **Object Pool Pattern**. This avoids expensive operations like `Instantiate()` and improves performance by reusing objects from a pool.

---

### 3. **Observer Pattern**
The **Observer Pattern** is used extensively to manage communication between systems. Key implementations include:
- **Health System:** Notifies all observers when the character's health changes.
- **Skill System:** Notifies relevant components when a skill is activated or deactivated.

---

## Why These Patterns?
- **Dependency Injection:** Ensures scalability and optimizes memory usage for mobile platforms.
- **Object Pooling:** Reduces CPU overhead by reusing objects instead of instantiating them repeatedly.
- **Observer Pattern:** Provides a clean and efficient way to handle communication between systems.

---

## How to Use
1. Clone the repository.
2. Open the project in Unity (version 2023.2.20f1 or higher).
3. Navigate to the `BaseScene` and play.
4. Check the `Scripts` folder for detailed implementations of the patterns and systems.


---