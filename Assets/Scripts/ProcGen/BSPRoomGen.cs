using System;
using System.ComponentModel;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
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
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(worldSize, 1, worldSize));
        Gizmos.DrawSphere(Vector3.zero, 1f);
    }

    [ContextMenu("Generate Rooms")]
    public void GenerateRooms()
    {
        HealthCheck();
        Debug.Log("<color=cyan><b>Starting room Generation</b></color>");

        var sampleRect = new RoomRect(Vector3.zero, worldSize, worldSize);
        var partition = SpacePartition.SplitSpace(sampleRect, minRoomSideSize);
        RenderPartitions(partition);
    }

    private List<RoomRect> GeneratePartitions()
    {
        return new();
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
        var rooms = partition.GetRooms();
        foreach (var room in rooms)
        {
            RenderRoom(new Room(room));
        }
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
}

public enum Side
{
    Left,
    Right,
    Bottom,
    Up
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
    public int Id;
    public RoomRect Area;
    public SpacePartition First;
    public SpacePartition Second;
    public bool IsLeaf => First == null && Second == null;

    public SpacePartition(RoomRect area, SpacePartition first = null, SpacePartition second = null, int id = 0)
    {
        Area = area;
        First = first;
        Second = second;
        Id = id;

        var bothNull = first == null && second == null;
        var neitherNull = first != null && second != null;
        Debug.Assert(bothNull || neitherNull, "You shouldn't have a tree with a single child");
    }

    public static SpacePartition SplitSpace(RoomRect area, float minRoomSideSize, int id = 0)
    {
        var root = new SpacePartition(area);
        root.Id = id;
        
        var result = SplitArea(area, minRoomSideSize);
        if (result is (var first, var second))
        {
            root.First = SplitSpace(first, minRoomSideSize, id+1);
            root.Second = SplitSpace(second, minRoomSideSize, root.First.Id + 1);
        }

        return root;
    }

    public static (RoomRect, RoomRect)? SplitArea(RoomRect area, float minRoomSideSize)
    {
        // If the room is too small to split, just return null
        if (area.Width < minRoomSideSize || area.Height < minRoomSideSize)
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
                if (area.Width <= minSideSize)
                    return null;

                // These are the new lengths of the new rooms
                var w1 = Random.Range(minRoomSideSize, area.Width - minRoomSideSize);
                var w2 = area.Width - w1;

                // Compute the new position of each room 
                var p1 = area.Position;
                var p2 = area.Position + new Vector2(w1, 0);

                return (new RoomRect(p1, w1, area.Height), new RoomRect(p2, w2, area.Height));
            }
            else
            {
                // If the side is too small, don't cut
                // TODO: retry with the other side
                var minSideSize = 2 * minRoomSideSize;
                if (area.Height <= minSideSize)
                    return null;

                // These are the new lengths of the new rooms
                var h1 = Random.Range(minRoomSideSize, area.Height - minRoomSideSize);
                var h2 = area.Height - h1;

                // Compute the new position of each room 
                var p1 = area.Position;
                var p2 = area.Position + new Vector2(0, h1);

                return (new RoomRect(p1, area.Width, h1), new RoomRect(p2, area.Width, h2));
            }
        }

        return null;
    }

    /// <summary>
    ///  Get rooms, just leafs 
    /// </summary>
    /// <returns></returns>
    public List<RoomRect> GetRooms()
    {
        var result = new List<RoomRect>();
        GetRoomsRec(result);

        return result;
    }

    private void GetRoomsRec(List<RoomRect> rooms)
    {
        if (IsLeaf)
        {
            rooms.Add(Area);
            return;
        }

        First?.GetRoomsRec(rooms);
        Second?.GetRoomsRec(rooms);
    }
}