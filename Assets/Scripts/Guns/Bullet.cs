using System;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Bullet : MonoBehaviour
{
    #region Inspector Variables

    [Description("How fast this bullet flies")]
    [SerializeField] private float maxSpeed = 10f;
    
    #endregion

    private void FixedUpdate()
    {
        var translation = maxSpeed * Time.deltaTime * transform.forward;
        transform.Translate(translation, Space.World);     
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collided!");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
