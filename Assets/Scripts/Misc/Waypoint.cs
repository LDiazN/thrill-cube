using System;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Color Color = Color.magenta;
    public bool DrawGizmos = true;


    private void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        
        Gizmos.color = Color;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

