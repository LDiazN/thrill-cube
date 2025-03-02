using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Bullet : MonoBehaviour
{
    #region Inspector Variables

    [Description("How fast this bullet flies")]
    [SerializeField] private float maxSpeed = 10f;
    
    #endregion

    private void Update()
    {
        var translation = maxSpeed * maxSpeed * Time.deltaTime * transform.forward;
        transform.Translate(translation);     
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collided!");
        Destroy(gameObject);
    }
}
