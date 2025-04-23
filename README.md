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

## Basic AI: Behavior Trees
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
### Knowledge Areas 

Enemies can belong to a knowledge area in the map to report the player to other enemies within the same area. 
This area is basically a **trigger collider** with data that it's considered shared between all enemies within this area. Enemies are registered and deregistered from this area when they enter and exit, 
so you don't have to manually assign the area to each enemy. 

This is useful to implement behaviors like "detect when someone is attacking a nearby ally". This demo provides a sample level that showcases this feature: 

![KnowledgeAreaTest](https://github.com/user-attachments/assets/5b969197-fa99-43c2-a10e-e1f7e74e3387)
--- 

## Procedural level generation
I wanted to go for an office floor type of level, where the space is a rectangular shape partitioned into rooms. This is an easy use case for the BSP algorithm. My implementation goes like this: 

### Parameters
- S: Min side size of each room. No room can have a side smaller than S
- P: Padding, it's a margin between the room limits and the area where you can generate content
- W: Wall thickness, how thick are the walls
- H: Hallway width, size of hallways (and therefore doors) 

### Algorithm
The first stage is the **space partition:**
1. Take a squared area in the space
2. Choose a cutting point at random, either vertically or horizontally, respecting that each half should be at the least of length S. If no such cut is possible, finish here
3. Set these two halves as children of this area, creating a binary tree
4. Repeat from 1. for both of the current area's children

This process will generate a binary tree of space partitions, where the actual rooms are defined by the leaves in this tree. 

The second stage is **connecting partitions:** each room will be rectangle inside its corresponding limits. You should be able to reach every room from every room, so that it's never impossible to win, while not connecting every room with every neighbor to make the traversal more interesting. To this end, I implemented the following connection logic: 

```
void ConnectRooms(PartitionTree tree) {
    if (isLeaf(tree))
        return; // Leaves are always connected
    // Make sure that your children are connected
    ConnectRooms(tree.first_child);
    ConnectRooms(tree.second_child);

    // Now connect leaves from first_child with leaves from second_child
    leaves_1 := GetChildren(tree.frst_child);
    leaves_2 := GetChildren(tree.second_child);
    for child_1 in leaves_1 {
        for child_2 in leaves_2 {
            if (IsNeighbor(child_1, child_2))
            {
                Connect(child_1, child_2);
                return;
            }
        }
    }
}
```

- `IsNeighbor` returns true when two partitions share a side
- `Connect` sets up a hallway between `child_1` and `child_2`
- `GetChildren` returns a list with all the leaves of a given `PartitionTree`

Note that to optimize `GetChildren` I used a DFS traversal algorithm to set up the start and finish time for each node, so that checking if a node is parent of another node is as efficient as: 
```
bool IsParent(PartitionTree node_1, PartitionTree node_2) {
    return node_1.start <= node_2.start && node_2.end <= node_1.end;
}
```
You can see the DFS traversal here: https://github.com/LDiazN/thrill-cube/blob/15733d7276cbc9af856991cf4c2d1fcedebeea43/Assets/Scripts/ProcGen/RoomGen.cs#L785-L799

The third stage is just **rendering the rooms**, which is a simpler process: 
1. Create and scale cubes properly for the floor, respecting the padding P
2. Create and scale cubes for the hallways, respecting the hallway width H and padding P
3. Create and scale cubes for the walls, respecting padding

The fourth stage is **populating the rooms**. For populating a room different rules apply depending on the area size.

Each room will find its corresponding parameters depending on its area. A room will always try to pick the configuration for the biggest area possible. 
For example, if we have a room with area 120 and configurations for sizes: 80, 100, 150, then our room will choose the configuration for size 100. 

These are the parameters that depend on the area size. Most of these parameters are described as a [min, max] interval to add some randomness: 
1. Amount of static props (decorative objects) 
2. Amount of shooter enemies
3. Amount of melee enemies, usually 0 for small rooms since they are hard to fight with little space
4. Amount of pickable guns, usually adjusted to have more guns where there are more melee enemies who can tank many shots
5. Amount of throwable weapons

With this in mind, populating a room works as follows:  
1. Find the smallest room by area and choose it as player start and reserve space for the player. No enemies can spawn in this room.
2. Populate enemies in this room according to parameters
3. Populate guns according to parameters. Always choose the max between amount of melees and amount of pickable guns, to ensure that there's always at the least one gun per melee enemy.
4. Populate throwable weapons

If there's not enough space to place an object, the object is skipped. Pickable guns and throwable weapons do not take space, so they can always be placed.


      

