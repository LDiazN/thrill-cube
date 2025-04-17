using System;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class PCGProp : MonoBehaviour
{
    #region Components

    private BoxCollider _collider;
    private BoxCollider collider => GetCollider();

    #endregion

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public RoomRect GetRect()
    {
        var min = collider.bounds.min;
        
        Vector2 position = Vector2.zero;
        if (gameObject.scene.IsValid())
            position = new Vector2(min.x, min.z);
        
        var size = collider.size;
        return new RoomRect(position, size.x, size.z);
    }

    /// <summary>
    /// Places this object in the specified position, but the pivot point
    /// will be the bottom left corner of this object's box collider
    /// </summary>
    /// <param name="position">Position to place this prop at</param>
    public void BLPlaceAt(Vector3 position)
    {
        var translation = position - collider.bounds.min;
        var finalPos = transform.position + translation;
        transform.position = finalPos;
    }

    private BoxCollider GetCollider()
    {
        if (_collider == null)
            _collider = GetComponent<BoxCollider>();

        return _collider;
    }
}
