# Cube With a Gun
![imagen](https://github.com/user-attachments/assets/844c1111-5b72-40d1-a52e-bb78ab3bdecd)

This is a sample project I made with **Unity 6** to explore AI programming and procedural generation in Unity. It's a simple **third person arena shooter** implementing the following mechanics:

1. **Basic AI** implemented with behavior trees, using [Unity 6's Behavior package](https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/index.html). The behavior tree controls the state changing logic while the actual behavior is implemented with custom action nodes. There are two basic enemies in this game: 
    - **Shooter Enemy**: Shoots to you from far away and dies fast
    - **Melee Enemy**: Charges against you and it's hard to kill
2. **Procedural level generation:** Implemented using the BSP algorithm, I wanted to generate random rooms with enemies that resemble office floors, and the [BSP algorithm](https://en.wikipedia.org/wiki/Binary_space_partitioning) makes this really easy
3. **Automatic Gameplay**: The player has an AI mode that you can activate at any time so that the game plays itself. Rather than converting the player in an NPC, this mode actually simulates user input. This is useful to design bots for multiplayer games or automatic testing  

--- 
## Unity Version and dependencies
Unity 6 (6000.0.40f1)

This project requires the following packages: 

1. Probuilder
2. Dotween
3. AI Navigation
4. Cinemachine
5. Behavior

## Behavior Trees
These are the behavior trees implemented for each enemy: 
### Melee Enemy
![imagen](https://github.com/user-attachments/assets/96c2d954-7434-454a-b1ba-f11120e4c124)
![imagen](https://github.com/user-attachments/assets/29195a54-2bb9-4b18-ae9b-dd18c7f088d1)

1. The enemy starts stationary, waiting to detect the player
2. It detects the player when:
    -   The player is close enough
    -   The player hurts this enemy
    -   Another enemy in the same knowledge area (more about this below) detected the player
3. It reports the player to other enemies in the same area
4. It continuously charges against the Player. If it's close enough, it hurts the player and waits a couple seconds before charging again

### Shooter Enemy
![imagen](https://github.com/user-attachments/assets/4b48331e-87f0-4181-af14-2f25b58cf302)

![imagen](https://github.com/user-attachments/assets/d3cc32b2-28c1-4a4e-a062-c1a2d1853306)

This enemy has two states: **patrolling and attacking**
1. **Patrolling:** It walks around a specified set of waypoints while checking if it can detect the player. Similarly to the previous enemy, it detects the player when:
    - The player is close enough
    - The player hurts this enemy
    - Another enemy in the same knowledge area (more about this below) detected the player
    - Once the player is detected, this enemy reports the player to its knowledge area and sets its state to "attacking"
2. **Attacking:**
    1. The enemy tries to close distance with the player
    2. Once in proper distance, it looks towards the player 
    3. Then it shoots and waits for the recoil before attacking again
## Knowledge Areas 

Enemies can belong to a knowledge area in the map to report the player to other enemies within the same area. 
This area is basically a **trigger collider** with data that it's considered shared between all enemies within this area. Enemies are registered and deregistered from this area when they enter and exit, 
so you don't have to manually assign the area to each enemy. 

This is useful to implement behaviors like "detect when someone is attacking a nearby ally". This demo provides a sample level that showcases this feature: 

![KnowledgeAreaTest](https://github.com/user-attachments/assets/5b969197-fa99-43c2-a10e-e1f7e74e3387)

