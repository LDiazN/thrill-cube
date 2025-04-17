using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Generates rooms using the BSP algorithm
/// </summary>
public class RoomGen : MonoBehaviour
{
    #region Inspector Properties

    [Header("Room Spec")] [SerializeField] private float minRoomSideSize = 5;

    [Description("How much distance between the edges of the square containing the room and the actual room")]
    [SerializeField]
    private float padding = 1;

    [SerializeField] private float wallThickness = 0.5f;

    [SerializeField] private float wallHeight = 6f;

    [SerializeField] private float hallwayWidth = 4f;

    [Header("World Spec")] [SerializeField]
    private float worldSize = 100;

    [Header("Props and enemies")] 
    [Description("Static props, like desk, chairs, computers")]
    [SerializeField]
    private List<PCGProp> props;

    [SerializeField]
    private PCGProp shooterEnemy;
    
    [SerializeField]
    private PCGProp meleeEnemy;

    [SerializeField] private List<PCGProp> guns;

    [Description("Objects that can be picked and make damage")]
    [SerializeField] private List<PCGProp> pickableObjects;

    [Description("Sometimes placing a prop in the room can have conflicts with other already placed props. " +
                 "This is the amount of times to retry if it fails to place a prop")]
    [SerializeField]
    [Min(1)]
    private int placementRetries = 10;
    
    [Header("Settings")] [SerializeField] private RoomBlock blockPrefab;

    #endregion

    #region Internal State

    private List<RoomBlock> _blocks = new();
    private List<GameObject> _props = new();

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero + new Vector3(worldSize / 2f, 0, worldSize / 2f),
            new Vector3(worldSize, 1, worldSize));
        Gizmos.DrawSphere(Vector3.zero, 1f);
    }

    [ContextMenu("Generate Rooms")]
    public void GenerateRooms()
    {
        HealthCheck();
        Debug.Log("<color=cyan><b>Starting room Generation</b></color>");

        var sampleRect = new RoomRect(Vector3.zero, worldSize, worldSize);

        // Step 1: Generate partitions
        var partition = SpacePartition.PartitionSpace(sampleRect, minRoomSideSize);
        Debug.Log($"<color=cyan>Generated <b>{partition.Context.RoomPartitions.Count}</b> rooms</color>");

        // Step 2.1: Set up children so we can find parents easily 
        Debug.Log($"<color=cyan>Generating connections...</color>");
        partition.SetUpChildren();
        // Step 2.2: Connect partitions so that every room is reachable
        partition.ConnectPartition(2 * padding + 2 * wallThickness);

        // Step 3: Render rooms, generate floors, walls, and hallways
        Debug.Log($"<color=cyan>Rendering world...</color>");
        RenderPartitions(partition);

        // Step 4: Populate rooms with props and enemies
        Debug.Log($"<color=cyan>Populating rooms...</color>");
        PopulateRooms(partition.GetRooms());
        
        Debug.Log($"<color=green>Room generation successfully finished!</color>");
    }

    private void PopulateRooms(List<Room> rooms)
    {
        foreach (var room in rooms)
            PopulateRoom(room);
    }

    private void PopulateRoom(in Room room)
    {
        var usableArea = room.UsableArea(padding, wallThickness);
        var propRects = new List<RoomRect>();
        
        // 1. Place static props according to room size
        PlaceStaticProps(usableArea, propRects);
    }

    private void PlaceStaticProps(RoomRect usableArea, List<RoomRect> placedProps)
    {
        var roomProps = props.Where(prop => prop.GetRect().CanFitIn(usableArea)).ToList();
        
        // If no prop fits in this room, just return
        if (!roomProps.Any())
            return;
        
        // TODO Choose this number dynamically depending on room size
        const int nProps = 3;
        for (var i = 0; i < nProps; i++)
        {
            // Choose a prop randomly
            var propIdx = Random.Range(0, roomProps.Count());
            var prop = roomProps[propIdx];
            var rect = prop.GetRect();

            // Try to find a position for this prop
            bool placementFound = false;
            Vector2 position = Vector2.zero;
            for (var retry = 0; retry < placementRetries && !placementFound; retry++)
            {
                position = ChooseRandomPositionInsideOf(usableArea, rect);
                rect.Position = position;
                
                // Check if the placement has conflicts with other props
                placementFound = CanPlace(rect, placedProps);
            }
            
            // We failed to place this prop, skip it
            if (!placementFound)
                continue;

            // Instantiate object and place it in the specified position
            var propObj = Instantiate(prop, Vector3.zero, Quaternion.identity);
            propObj.BLPlaceAt(new Vector3(position.x, 0, position.y));
            _props.Add(propObj.gameObject);
            
            // Add the new prop to the list 
            var propRect = propObj.GetRect();
            placedProps.Add(propRect);
        }
    }

    private static bool CanPlace(in RoomRect rect, List<RoomRect> placedProps)
    {
        foreach(var propRect in placedProps)
            if (propRect.Intersects(rect))
                return false;
        return true;
    }

    private static Vector2 ChooseRandomPositionInsideOf(in RoomRect container, in RoomRect other)
    {
        var contMinX = container.Position.x;
        var contMaxX = contMinX + container.Width;
        var contMinY = container.Position.y;
        var contMaxY = contMinY + container.Height;
        
        return new Vector2(
            Random.Range(contMinX, contMaxX - other.Width),
            Random.Range(contMinY, contMaxY - other.Height)
            );
    }

    [ContextMenu("Reset Rooms")]
    public void Reset()
    {
        Debug.Log("<color=red><b>Resetting current rooms</b></color>");
        foreach (var block in _blocks)
            Destroy(block.gameObject);

        _blocks.Clear();

        foreach (var obj in _props)
            Destroy(obj);
        
        _props.Clear();
    }

    /// <summary>Check the parameters to see if they make sense</summary> 
    private void HealthCheck()
    {
        Debug.Log("<color=green><b>Checking parameters consistency...</b></color>");
        var sizesOk = 2 * padding + 2 * wallThickness < minRoomSideSize;

        Debug.Assert(sizesOk,
            "Padding and wall thickness should be less than the overall minimal size of a room");

        var hallwayWidthOk = hallwayWidth < minRoomSideSize - (2 * padding + 2 * wallThickness);
        Debug.Assert(hallwayWidthOk, "Hallway width is too big for the current room dimensions");

        Debug.Log("<color=green><b>Everything OK!</b></color>");
    }

    private void RenderRoom(in Room room)
    {
        var floor = Instantiate(blockPrefab, new Vector3(room.Area.Position.x, 0, room.Area.Position.y),
            Quaternion.identity);

        floor.transform.position += new Vector3(padding, 0, padding);
        floor.SetSize(new Vector3(room.Area.Width - 2 * padding, 0.5f, room.Area.Height - 2 * padding));

        _blocks.Add(floor);

        RenderWalls(room);
    }

    private void RenderWalls(in Room room)
    {
        // Render Left Wall
        var sides = new[] { Side.Left, Side.Right, Side.Bottom, Side.Top };

        foreach (var side in sides)
        {
            var isVertical = side is Side.Left or Side.Right;
            var blCorner = room.Area.WorldPosition;
            blCorner.x += padding;
            blCorner.z += padding;

            if (side is Side.Right)
                blCorner.x += room.Area.Width - 2 * padding;
            if (side is Side.Top)
                blCorner.z += room.Area.Height - 2 * padding;

            // Get the length of the side depending on which side it is
            float sideLength = isVertical ? room.Area.Height : room.Area.Width;

            var doors = room.Doors.Where(door => door.Side == side).OrderBy(door => door.Start).ToList();
            float nextStart = 0;
            var newCorner = blCorner;
            foreach (var door in doors)
            {
                var end = door.Start;
                var length = end - nextStart;
                RenderWall(newCorner, length, isVertical);
                nextStart = end + door.Length;

                // move corner past the door
                if (isVertical)
                    newCorner.z += length + door.Length;
                else
                    newCorner.x += length + door.Length;
            }

            // Render the last wall to finish this side
            RenderWall(newCorner, (sideLength - 2 * padding) - nextStart, isVertical);
        }
    }

    private void RenderWall(Vector3 blCorner, float length, bool isVertical)
    {
        var size = Vector3.one;
        if (isVertical)
        {
            size.z = length;
            size.x = wallThickness;
        }
        else
        {
            size.x = length;
            size.z = wallThickness;
        }

        size.y = wallHeight;

        var block = Instantiate(blockPrefab, blCorner, Quaternion.identity);
        block.transform.localScale = size;
        _blocks.Add(block);
    }

    private void RenderPartitions(SpacePartition partition)
    {
        var rooms = partition.GetRooms(2 * padding + 2 * wallThickness, hallwayWidth);
        RenderRooms(rooms);
        RenderHallways(partition.Context.Hallways);
    }

    private void RenderRooms(List<Room> rooms)
    {
        foreach (var room in rooms)
            RenderRoom(room);
    }

    private void RenderHallways(List<Hallway> hallways)
    {
        // Try to draw the hallways in the specified connection points
        foreach (var hallway in hallways)
        {
            var initPosition = hallway.Position;
            var size = Vector3.one;
            size.y = 0.5f;

            if (hallway.IsHorizontal)
            {
                initPosition.x -= padding;
                initPosition.z += padding;
                size.x = 2 * padding;
                size.z = hallwayWidth;
            }
            else
            {
                initPosition.z -= padding;
                initPosition.x += padding;
                size.z = 2 * padding;
                size.x = hallwayWidth;
            }

            var hallwayBlock = Instantiate(blockPrefab, initPosition, Quaternion.identity);
            hallwayBlock.transform.localScale = size;
            _blocks.Add(hallwayBlock);
            RenderHallwayWalls(hallway);
        }
    }

    private void RenderHallwayWalls(in Hallway hallway)
    {
        // Hallways have two walls, one to each side of its corresponding direction
        var startPosition = hallway.Position;
        if (hallway.IsHorizontal)
        {
            startPosition.x -= padding;
            startPosition.z += padding;
        }
        else
        {
            startPosition.z -= padding;
            startPosition.x += padding;
        }

        // startPosition.x += padding;
        // startPosition.z += padding;
        RenderWall(startPosition, 2 * padding + wallThickness, hallway.IsVertical);

        if (hallway.IsVertical)
            startPosition.x += hallwayWidth;
        else
            startPosition.z += hallwayWidth;

        RenderWall(startPosition, 2 * padding + wallThickness, hallway.IsVertical);
    }
}


/// <summary>
///  A binary tree used for the BSP algorithm
/// </summary>
public class SpacePartition
{
    public int Start; // Time of visit of your first node in this subtree, aka yourself
    public int End; // Time of visit  of your last node in this subtree, aka your last children
    public PartitionContext Context;
    public RoomRect Rect;
    public SpacePartition First;
    public SpacePartition Second;
    public bool IsLeaf => First == null && Second == null;
    public int RoomIndex = -1;

    // Only setted when it's a leaf/room
    public List<SpacePartition> ConnectedRooms = null;

    private void ConnectTo(SpacePartition other)
    {
        ConnectedRooms ??= new();
        ConnectedRooms.Add(other);
    }

    public SpacePartition(RoomRect rect, SpacePartition first = null, SpacePartition second = null)
    {
        Rect = rect;
        First = first;
        Second = second;

        var bothNull = first == null && second == null;
        var neitherNull = first != null && second != null;
        Debug.Assert(bothNull || neitherNull, "You shouldn't have a tree with a single child");
    }

    public static SpacePartition PartitionSpace(RoomRect rect, float minRoomSideSize, PartitionContext context = null)
    {
        context ??= new PartitionContext();
        var current = new SpacePartition(rect)
        {
            Context = context
        };

        var result = SplitRect(rect, minRoomSideSize);

        // If no partition was possible, this is a leaf, return
        if (result is not var (first, second))
        {
            current.RoomIndex = context.RoomPartitions.Count;
            context.RoomPartitions.Add(current);
            return current;
        }

        current.First = PartitionSpace(first, minRoomSideSize, context);
        current.Second = PartitionSpace(second, minRoomSideSize, context);

        return current;
    }

    static (RoomRect, RoomRect)? SplitRect(RoomRect rect, float minRoomSideSize)
    {
        // If the room is too small to split, just return null
        if (rect.Width < minRoomSideSize || rect.Height < minRoomSideSize)
            return null;

        // Choose in which direction to cut, vertical or horizontal
        var firstTry = Random.value > 0.5f;
        bool[] tries;
        tries = new[] { firstTry, !firstTry };

        // Choose a cutting point
        foreach (var isVertical in tries)
        {
            if (isVertical)
            {
                // If the side is too small, don't cut
                // TODO: retry with the other side
                var minSideSize = 2 * minRoomSideSize;
                if (rect.Width <= minSideSize)
                    return null;

                // These are the new lengths of the new rooms
                var w1 = Random.Range(minRoomSideSize, rect.Width - minRoomSideSize);
                var w2 = rect.Width - w1;

                // Compute the new position of each room 
                var p1 = rect.Position;
                var p2 = rect.Position + new Vector2(w1, 0);

                return (new RoomRect(p1, w1, rect.Height), new RoomRect(p2, w2, rect.Height));
            }
            else
            {
                // If the side is too small, don't cut
                // TODO: retry with the other side
                var minSideSize = 2 * minRoomSideSize;
                if (rect.Height <= minSideSize)
                    return null;

                // These are the new lengths of the new rooms
                var h1 = Random.Range(minRoomSideSize, rect.Height - minRoomSideSize);
                var h2 = rect.Height - h1;

                // Compute the new position of each room 
                var p1 = rect.Position;
                var p2 = rect.Position + new Vector2(0, h1);

                return (new RoomRect(p1, rect.Width, h1), new RoomRect(p2, rect.Width, h2));
            }
        }

        return null;
    }

    /// <summary>
    ///  Get rooms, just leafs 
    /// </summary>
    /// <returns></returns>
    public List<Room> GetRooms(float hallwayMargins, float hallwayWidth)
    {
        if (Context.Rooms != null)
            return Context.Rooms;

        // Rooms not yet done, create them
        BuildRooms(hallwayMargins, hallwayWidth);

        return Context.Rooms;
    }

    public List<Room> GetRooms() => Context.Rooms;

    private void BuildRooms(float margin, float hallwayWidth)
    {
        var rooms = new List<Room>();
        foreach (var room in Context.RoomPartitions)
        {
            rooms.Add(new Room(room.Rect));
        }

        var roomConnections = GetRoomConnections();
        var hallways = new List<Hallway>();

        foreach (var (p1, p2) in roomConnections)
        {
            var maybeSharedEdge = p1.Rect.ShareEdge(p2.Rect, margin);
            Debug.Assert(maybeSharedEdge != null, "Should be connected!");
            var sharedEdge = (RoomRect.EdgeShare)maybeSharedEdge;
            Vector2 startPoint = Vector2.zero;
            float randomStart = ChooseHallwayStart(sharedEdge.Start, sharedEdge.Length, margin, hallwayWidth);
            float rs2;

            // Set up a different starting point depending on the side
            switch (sharedEdge.Side)
            {
                case Side.Left:
                    startPoint = new(p1.Rect.Position.x, p1.Rect.Position.y + randomStart);
                    rs2 = p1.Rect.Position.y + randomStart - p2.Rect.Position.y;
                    break;
                case Side.Right:
                    startPoint = new(p1.Rect.Position.x + p1.Rect.Width, p1.Rect.Position.y + randomStart);
                    rs2 = p1.Rect.Position.y + randomStart - p2.Rect.Position.y;
                    break;
                case Side.Bottom:
                    startPoint = new(p1.Rect.Position.x + randomStart, p1.Rect.Position.y);
                    rs2 = p1.Rect.Position.x + randomStart - p2.Rect.Position.x;
                    break;
                case Side.Top:
                    startPoint = new(p1.Rect.Position.x + randomStart, p1.Rect.Position.y + p1.Rect.Height);
                    rs2 = p1.Rect.Position.x + randomStart - p2.Rect.Position.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Add new hallway
            hallways.Add(new Hallway
            {
                IsVertical = sharedEdge.Side is Side.Bottom or Side.Top,
                Position = new Vector3(startPoint.x, 0, startPoint.y)
            });

            // Add doors for each room 
            rooms[p1.RoomIndex].Doors.Add(new Door
            {
                Length = hallwayWidth,
                Side = sharedEdge.Side,
                Start = randomStart
            });

            // We have to compute this again but the other way round 
            var maybe2SharedEdge = p2.Rect.ShareEdge(p1.Rect, margin);
            Debug.Assert(maybe2SharedEdge != null, "This should also share and edge!");

            rooms[p2.RoomIndex].Doors.Add(new Door
            {
                Length = hallwayWidth,
                Side = OppositeSide(sharedEdge.Side),
                Start = rs2
            });
        }

        Context.Hallways = hallways;
        Context.Rooms = rooms;
    }

    private HashSet<(SpacePartition, SpacePartition)> GetRoomConnections()
    {
        HashSet<(SpacePartition, SpacePartition)> connections = new();

        // Find all the hallways you have to draw
        foreach (var room in Context.RoomPartitions)
        foreach (var nb in room.ConnectedRooms)
            if (!connections.Contains((nb, room)))
                connections.Add((room, nb));
        return connections;
    }

    private float ChooseHallwayStart(float start, float availableSize, float topMargin, float hallwayWidth)
    {
        return Random.Range(start, start + availableSize - topMargin - hallwayWidth);
    }

    /// <summary>
    /// Will set up start and finish count of children nodes
    /// so we can query parents very fast 
    /// </summary>
    public void SetUpChildren() => SetUpChildrenRec();

    private int SetUpChildrenRec(int start = 0)
    {
        Start = start;
        start++;
        if (First != null)
        {
            start = First.SetUpChildrenRec(start);
            Debug.Assert(Second != null, "Should not have just one child");
            End = Second.SetUpChildrenRec(start) + 1;
            return End;
        }

        End = start;
        return start;
    }

    private bool IsMyChild(SpacePartition other) => Start <= other.Start && other.End <= End;

    public void ConnectPartition(float minIntersectionSize)
    {
        if (IsLeaf)
            return;

        First.ConnectPartition(minIntersectionSize);
        Second.ConnectPartition(minIntersectionSize);
        ConnectRooms(First, Second, minIntersectionSize);
    }

    private static void ConnectRooms(SpacePartition p1, SpacePartition p2, float minIntersectionSize)
    {
        var childrenP1 = p1.GetChildren();
        var childrenP2 = p2.GetChildren();
        var connected = false;
        foreach (var child in childrenP1.TakeWhile(child => !connected))
        {
            foreach (var nb in child.GetNeighbors(minIntersectionSize).Where(nb => childrenP2.Contains(nb)))
            {
                // Connect child to nb 
                child.ConnectTo(nb);
                nb.ConnectTo(child);
                connected = true;
                break;
            }
        }
    }

    private List<SpacePartition> GetNeighbors(float minIntersectionSize)
    {
        return Context.RoomPartitions.Where(room => IsNeighbor(room, minIntersectionSize)).ToList();
    }

    private bool IsNeighbor(SpacePartition other, float minIntersectionSize)
    {
        Debug.Assert(minIntersectionSize > 0, "size of intersection should be possitive to make sense");
        return Rect.ShareEdge(other.Rect, minIntersectionSize) != null;
    }

    private List<SpacePartition> GetChildren()
    {
        return Context.RoomPartitions.Where(IsMyChild).ToList();
    }

    public static Side OppositeSide(Side side)
    {
        switch (side)
        {
            case Side.Left:
                return Side.Right;
            case Side.Right:
                return Side.Left;
            case Side.Bottom:
                return Side.Top;
            case Side.Top:
                return Side.Bottom;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }
}

public class PartitionContext
{
    // Leaf nodes that correspond to actual rooms
    public List<SpacePartition> RoomPartitions = new();

    public List<Hallway> Hallways = new();

    // Actual rooms, set up after all the generation process is done
    public List<Room> Rooms;
}