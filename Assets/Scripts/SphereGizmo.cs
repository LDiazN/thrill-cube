using System;
using UnityEngine;

public class SphereGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Ray r = new Ray(transform.position, transform.forward);
        Gizmos.color = Color.blue;
        
        Gizmos.DrawRay(r);
    }
}
