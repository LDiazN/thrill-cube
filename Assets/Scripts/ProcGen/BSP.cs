using System.Collections.Generic;
using UnityEngine;

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
    
    public override string ToString()
    {
        return $"[Pos: {Position} | Width: {Width} | Height: {Height}]";
    }

    public readonly bool CanFitIn(in RoomRect other)
    {
        var otherMaxX = other.Width;
        var otherMaxY = other.Height;
        
        var maxX = Width;
        var maxY = Height;

        return  maxX <= otherMaxX && maxY <= otherMaxY;
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

    public readonly RoomRect UsableArea(float padding, float wallThickness)
    {
        var margin = padding + wallThickness;
        return new RoomRect(
            Area.Position + margin * Vector2.one, 
            Area.Width - 2 * margin, 
            Area.Height - 2 * margin
            );
    }

}
