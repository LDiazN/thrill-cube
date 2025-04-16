using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

public class BSPRoomGen : MonoBehaviour
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

    [Header("Settings")] [SerializeField] private RoomBlock blockPrefab;

    #endregion

    #region Internal State

    private List<RoomBlock> _blocks = new();

    #endregion

    private void Start()
    {
        // DELETE ME LATER
        GenerateRooms();
    }

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

        // Step 3: Render rooms
        RenderPartitions(partition);
    }

    [ContextMenu("Reset Rooms")]
    public void Reset()
    {
        Debug.Log("<color=red><b>Resetting current rooms</b></color>");
        foreach (var block in _blocks)
        {
            Destroy(block.gameObject);
        }

        _blocks.Clear();
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

    private void RenderRoom(Room block)
    {
        var floor = Instantiate(blockPrefab, new Vector3(block.Area.Position.x, 0, block.Area.Position.y),
            Quaternion.identity);

        floor.transform.position += new Vector3(padding, 0, padding);
        floor.SetSize(new Vector3(block.Area.Width - 2 * padding, 0.5f, block.Area.Height - 2 * padding));

        _blocks.Add(floor);
    }

    private void RenderPartitions(SpacePartition partition)
    {
        RenderRooms(partition);
        RenderHallways(partition);
    }

    private void RenderRooms(SpacePartition partition)
    {
        var rooms = partition.GetRooms(2 * padding + 2 * wallThickness, hallwayWidth);
        foreach (var room in rooms)
            RenderRoom(room);
    }

    private void RenderHallways(SpacePartition partition)
    {
        HashSet<(SpacePartition, SpacePartition)> connections = new();

        // Find all the hallways you have to draw
        foreach (var room in partition.Context.RoomPartitions)
        foreach (var nb in room.ConnectedRooms)
            if (!connections.Contains((nb, room)))
                connections.Add((room, nb));

        // Try to draw the hallways in the specified connection points
        foreach (var (p1, p2) in connections)
        {
            var maybeSharedEdge = p1.Rect.ShareEdge(p2.Rect, 2 * padding + 2 * wallThickness);
            Debug.Assert(maybeSharedEdge != null, "Should be connected!");
            var sharedEdge = (RoomRect.EdgeShare)maybeSharedEdge;
            Vector3 startPoint = Vector3.zero;
            Vector3 size = Vector3.one;

            switch (sharedEdge.Side)
            {
                case Side.Left:
                    startPoint = new(p1.Rect.Position.x - padding, 0,
                        p1.Rect.Position.y + ChooseHallwayStart(sharedEdge.Start, sharedEdge.Length));
                    size = new(2 * padding, 0.5f, hallwayWidth);
                    break;
                case Side.Right:
                    startPoint = new(p1.Rect.Position.x + p1.Rect.Width - padding, 0,
                        p1.Rect.Position.y + ChooseHallwayStart(sharedEdge.Start, sharedEdge.Length));
                    size = new(2 * padding, 0.5f, hallwayWidth);
                    break;
                case Side.Bottom:
                    startPoint = new(p1.Rect.Position.x + ChooseHallwayStart(sharedEdge.Start, sharedEdge.Length), 0,
                        p1.Rect.Position.y - padding);
                    size = new(hallwayWidth, 0.5f, 2 * padding);
                    break;
                case Side.Top:
                    startPoint = new(p1.Rect.Position.x + ChooseHallwayStart(sharedEdge.Start, sharedEdge.Length), 0,
                        p1.Rect.Position.y + p1.Rect.Height - padding);
                    size = new(hallwayWidth, 0.5f, 2 * padding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var hallway = Instantiate(blockPrefab, startPoint, Quaternion.identity);
            hallway.transform.localScale = size;
        }
    }

    private float ChooseHallwayStart(float start, float availableSize)
    {
        var offset = padding + wallHeight + hallwayWidth / 2f;
        return Random.Range(start + offset, start + availableSize - offset);
    }
}

/// <summary>
/// A specifying a room's dimensions.
/// We only generate rooms in 2D, so we only consider width and height coordinates
/// </summary>
public struct RoomRect
{
    public Vector2 Position;
    public float Width;
    public float Height;

    public Vector3 WorldPosition => new Vector3(Position.x, 0, Position.y);

    public RoomRect(Vector2 position, float width, float height)
    {
        Position = position;
        Width = width;
        Height = height;
    }

    public EdgeShare? ShareEdge(RoomRect other, float minSharedLength)
    {
        var a = this;
        var b = other;

        float aLeft = a.Position.x;
        float aRight = a.Position.x + a.Width;
        float aBottom = a.Position.y;
        float aTop = a.Position.y + a.Height;

        float bLeft = b.Position.x;
        float bRight = b.Position.x + b.Width;
        float bBottom = b.Position.y;
        float bTop = b.Position.y + b.Height;


        bool rAlign = Mathf.Approximately(aRight, bLeft);
        bool lAlign = Mathf.Approximately(aLeft, bRight);
        bool vIntersect = (rAlign || lAlign);

        bool tAlign = Mathf.Approximately(aTop, bBottom);
        bool bAlign = Mathf.Approximately(aBottom, bTop);
        bool hIntersect = (tAlign || bAlign);

        // No side intersection
        if (!vIntersect && !hIntersect)
            return null;

        EdgeShare result = new();
        if (vIntersect)
        {
            result.Side = rAlign ? Side.Right : Side.Left;
            result.Start = Mathf.Max(0, bBottom - aBottom);
            result.Length = Mathf.Min(bTop, aTop) - Mathf.Max(aBottom, bBottom);
        }
        else
        {
            result.Side = bAlign ? Side.Bottom : Side.Top;
            result.Start = Mathf.Max(0, bLeft - aLeft);
            result.Length = Mathf.Min(bRight, aRight) - Mathf.Max(aLeft, bLeft);
        }

        if (result.Length < minSharedLength)
            return null;

        return result;
    }

    public struct EdgeShare
    {
        public Side Side;
        public float Start;
        public float Length;
    }
}

public enum Side
{
    Left,
    Right,
    Bottom,
    Top
}

public struct Hallway
{
    // In world space, bottom left corner, ignores padding and walls
    public Vector3 Position;
    public bool IsVertical;
    public bool IsHorizontal => !IsVertical;
}

public struct Door
{
    public Side Side;
    public float Start;
    public float Length;
}

public struct Room
{
    public RoomRect Area;
    public List<Door> Doors;

    public Room(RoomRect area, List<Door> doors = null)
    {
        doors ??= new List<Door>();

        Area = area;
        Doors = doors;
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

            // Set up a different starting point depending on the side
            switch (sharedEdge.Side)
            {
                case Side.Left:
                    startPoint = new(p1.Rect.Position.x, p1.Rect.Position.y + randomStart);
                    break;
                case Side.Right:
                    startPoint = new(p1.Rect.Position.x + p1.Rect.Width, p1.Rect.Position.y + randomStart);
                    break;
                case Side.Bottom:
                    startPoint = new(p1.Rect.Position.x + randomStart, p1.Rect.Position.y);
                    break;
                case Side.Top:
                    startPoint = new(p1.Rect.Position.x + randomStart, p1.Rect.Position.y + p1.Rect.Height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Add new hallway
            hallways.Add(new Hallway
            {
                IsVertical = sharedEdge.Side is Side.Left or Side.Right,
                Position = new Vector3(startPoint.x, 0, startPoint.y)
            });
            
            // Add doors for each room 
            rooms[p1.RoomIndex].Doors.Add(new Door
            {
                Length = hallwayWidth, 
                Side = sharedEdge.Side, 
                Start = sharedEdge.Start
            });
            
            // We have to compute this again but the other way round 
            var maybe2SharedEdge = p2.Rect.ShareEdge(p1.Rect, margin);
            Debug.Assert(maybe2SharedEdge != null, "This should also share and edge!");
            var p2SharedEdge = (RoomRect.EdgeShare) maybe2SharedEdge;
            
            rooms[p2.RoomIndex].Doors.Add(new Door
            {
                Length = hallwayWidth, 
                Side = OppositeSide(sharedEdge.Side),
                Start = p2SharedEdge.Start
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