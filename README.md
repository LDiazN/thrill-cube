# Cube With a Gun

![CubeWithAGunMenu](https://github.com/user-attachments/assets/2cb3a74e-514e-4b08-951e-6e898a96999f)


This is a sample project I made with **Unity 6** to explore AI programming and procedural generation in Unity. It's a simple **third person arena shooter** implementing the following mechanics:

1. **Basic AI** implemented with behavior trees, using [Unity 6's Behavior package](https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/index.html). The behavior tree controls the state changing logic while the actual behavior is implemented with custom action nodes. There are two basic enemies in this game: 
    - **Shooter Enemy**: Shoots to you from far away and dies fast
    - **Melee Enemy**: Charges against you and it's hard to kill
2. **Procedural level generation:** Implemented using the BSP algorithm, I wanted to generate random rooms with enemies that resemble office floors, and the [BSP algorithm](https://en.wikipedia.org/wiki/Binary_space_partitioning) makes this really easy
3. **Automatic Gameplay**: The player has an AI mode that you can activate at any time so that the game plays itself. Rather than converting the player in an NPC, this mode actually simulates user input. This is useful to design bots for multiplayer games or automatic testing  

## Table of contents
- [Unity version and dependencies](#unity-version-and-dependencies)
- [Levels](#levels)
- [Basic AI: Behavior Trees](#basic-ai-behavior-trees)
  - [Melee Enemy](#melee-enemy)
  - [Shooter Enemy](#shooter-enemy)
  - [Knowledge Areas](#knowledge-areas)
- [Procedural level generation](#procedural-level-generation)
  - [Parameters](#parameters)
  - [Algorithm](#algorithm)
- [Autopilot](#autopilot)

--- 
## Unity version and dependencies
Unity 6 (6000.0.40f1)

This project requires the following packages: 

1. Probuilder
2. Dotween
3. AI Navigation
4. Cinemachine
5. Behavior

## Levels

The main build profile provides the following levels: 
1. **Sample Level**: A simple level showcasing the fundamental game features and AI behavior. You have to kill all enemies to win 
2. **Random Rooms Level**: Same as the previous level, but this time rooms are auto generated every time the level loads. You have to kill every enemy to win the level and it provides more complex features like ammunition and throwable weapons.
3. **Knowledge Sharing Level**: A simple level showcasing the knowledge area feature for enemies
4. **BSP Demo**: A simple scene where you can play with the level generation system. You can change some parameters and inspect the generated level.
5. **Shooter Enemy Level:** A simple level to test the shooter enemy AI
6. **Autopilot Ghost example:** A simple scene used to test the autopilot feature. Press **tab** to give control to the AI. In this scene you can actually see the "ghost" that is controlling the AI behavior
7. **Sample Level (autopilot enabled):** The same as the sample level but is properly set up to allow the AI autopilot to work 

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
![ExampleFastGeneration](https://github.com/user-attachments/assets/721bcbc0-137c-4d1b-bc19-212b7e7f8f76)


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
- `Connect` sets up a hallway between `child_1` and `child_2`. It chooses randomly a position within the shared edge while respecting W, P and H 
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


| ![ExampleRoomEmpty](https://github.com/user-attachments/assets/51a298f4-e4ab-459a-88ef-8253aa7bc3aa) |![ExampleRoomProps](https://github.com/user-attachments/assets/fc69918f-56d5-4ac7-a846-b36bd8e5fa10) | ![ExampleRoomEnemies](https://github.com/user-attachments/assets/368eb8ac-3abb-412c-996d-9672e8204b76) | ![ExampleRoomFull](https://github.com/user-attachments/assets/341b3694-97d8-4ac8-b763-3a80028e872d) |
|:--:|:--:|:--:|:--:|
| Empty room, only the player start is placed | Static props added | Enemies Added | Pickables and weapons added |

You can test the level generation in the **"BSP Demo"** scene.

The **"Random Rooms Level"** scene is a playable level where you spawn in a randomly generated level with enemies you have to kill to win the game. Props are removed from this scene because they are annoying during the gameplay, rooms are also bigger and lesser to encourage fighting and reduce the need for exploration.

## Autopilot 

| ![ExampleImmortal - Trim](https://github.com/user-attachments/assets/ca5a6e89-085e-40b8-ba94-b7207911bdeb) |
|:--:|
| _Don't mess with the cube_ |

The player can give control to the AI at any moment by pressing the **tab** key. This will start the autopilot mode that tries to kill every enemy in the level automatically. You can also press **tab** again to regain control of the character. 

The AI mode is indicated by the blinking blue text at the top left corner of the screen: "AI control active".

I also wanted to avoid turning the player character into an NPC while in AI mode. I was looking for an approach that could emulate human input. My solution was using
a "ghost", and invisible and intangible object that would implement the actual AI, and a translator in the playable character that would translate its behavior to user input. 
The result looked something like this: 

![ExampleWithGhost](https://github.com/user-attachments/assets/02f15910-3f92-47bc-97ac-3b5ceb9cf572)

- The purple transparent cube is the "ghost" that implements the actual AI behavior
- When the player activates the AI control, user input is deactivated in the character and another components proceeds to translate the ghost's behavior to user input.
   - If the ghost goes forward, it translates it to the user pressing the forward button ("W")
   - If the robot decides that it wants to shoot an enemy, the translator computes the required horizontal and vertical mouse input to center the crosshair in the enemy and shoot only when it's sure that it will hit

This approach has its drawbacks. For example, since the player is "following" the ghost, it can get bugged on corners or act on the wrong information (the ghost has line of sight with the enemy but the player doesn't, for example). To mitigate these problems I reset the position of the ghost to the position of the player if they split appart too much. 

The AI itself is very simple, it's based on the idea that an human player is usually doing two things at the same time when playing this type of game: 
1. **Moving**: Avoiding dangers and chasing enemies
2. **Shooting:** Aiming towards enemies and shooting at the right moment.

Of course, the reality is more complex and these activities are closely related to each other, but I used it as an useful approximation.

This approach led to the following behavior tree: 

![imagen](https://github.com/user-attachments/assets/24d05b5d-2ea8-40d3-ac6c-41cac6bae1a7)

- It's always running two behaviors at the same time: Chase and shoot.
- Chase will choose the closest enemy (by nav mesh path length) and make the ghost navigate to a position close enough to shoot.
- Shoot will continously try to aim and shoot to the closest enemy

Unfortunately I didn't have the time to make a more complex movement behavior, so the player won't try to avoid dangers or look for another weapon if it runs out of ammo. But nevertheless it's still very interesting as it is. 

I mentioned that the closest enemy is chosen by **nav mesh path length instead of euclidian distance**. This is important because it prevents the player from prioritizing enemies in a neighbor room with a wall in the middle. But this is an expensive calculation, how do we do it without tanking the frame rate?

If we consider that the change in position between frames is always small, then it's not necessary to actually run it every frame. Instead, I compute the closest enemy **twice per second** and cache the result. The enemy is not always in sync but is beliavable enough.   

- The **Sample Level (autopilot enabled)** scene has an inmortal player with a gun with infinite ammo. It's intended to showcase the autopilot.
- The **Autopilot ghost example** scene has a visible ghost to show how the player follows its movements.

Remember to press **tab** to start the autopilot mode. You can press it again to regain control. 

![ExampleImmortal - Short(1)](https://github.com/user-attachments/assets/916b2f78-e1d9-4850-89c0-000ec2498219)



      

