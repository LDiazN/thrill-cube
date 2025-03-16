# Cube With a Gun
![imagen](https://github.com/user-attachments/assets/844c1111-5b72-40d1-a52e-bb78ab3bdecd)

This is a sample project I made with **Unity 6** to explore AI programming in Unity. It's a simple **third person arena shooter** with basic AI functionality.

There are two basic enemies in this game: 
- **Shooter Enemy**: Shoots to you from far away and dies fast
- **Melee Enemy**: Charges against you and it's hard to kill

The AI is mostly implemented using [Unity 6's Behavior package](https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/index.html). The behavior tree controls the state changing logic while the 
actual behavior is implemented with custom action nodes. 

--- 
## Unity Version
Unity 6 (6000.0.40f1)

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

