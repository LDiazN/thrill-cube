using UnityEngine;

/// <summary>
/// This class will manage shapes used to make a room.
/// This is mostly a wrapper over a cube with a different pivot point
/// </summary>
public class RoomBlock : MonoBehaviour
{
    public void SetSize(Vector3 size)
    {
        transform.localScale = size;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
