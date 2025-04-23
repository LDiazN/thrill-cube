using System.ComponentModel;
using UnityEngine;


public class PCGProp : MonoBehaviour
{
    #region Inspector Properties

    [Min(0.1f)]
    [SerializeField] private float height;
    [Min(0.1f)]
    [SerializeField] private float width;
    [Description("Is Y in world coordinates, used for placing the object in the world")]
    
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        var rect = GetRect();

        var dimensions = new Vector3(rect.Width, 1, rect.Height);
        var position = rect.WorldPosition;
        Gizmos.DrawWireCube(rect.WorldPosition + new Vector3(rect.Width/2, -1, rect.Height/2), dimensions);
    }

    public RoomRect GetRect()
    {
        Vector2 position = new (transform.position.x, transform.position.z);
        position.x -= width / 2;
        position.y -= height / 2;
        
        return new RoomRect(position, width, height);
    }

    /// <summary>
    /// Places this object in the specified position, but the pivot point
    /// will be the bottom left corner of this object's box collider
    /// </summary>
    /// <param name="position">Position to place this prop at</param>
    public void BLPlaceAt(Vector3 position)
    {
        Vector3 translation = new(width / 2, 0, height / 2);
        var finalPos = position + translation;
        finalPos.y += 1;
        transform.position = finalPos;
    }

}
