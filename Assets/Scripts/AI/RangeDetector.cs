using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Checks if the player is within the specified range
/// </summary>
public class RangeDetector : MonoBehaviour
{
    #region Inspector Properties

    [FormerlySerializedAs("name")] [Description("Just an editor name to know what this detector does")]
    public string detectorName;

    [Description("Player to check for range")] [SerializeField]
    private Player player;
    public Player Player => player;

    [Description("Range of detection around this enemy")] [SerializeField]
    [Min(0)]
    private float range;

    [Header("Gizmos")]
    [Description("Gizmos color")]
    [SerializeField] private Color gizmosColor = Color.red;

    [Description("If should draw gizmos for this object")]
    [SerializeField] private bool drawGizmos = true;

    #endregion


    private void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        }
    }

    public bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        // Check if within range
        var playerInRange = Vector3.SqrMagnitude(transform.position - player.transform.position) < range * range;
        if (!playerInRange)
            return false;
        
        // Check if visible
        var toPlayer = player.transform.position - transform.position;
        var hitSomething = Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, range);
        if (!hitSomething)
            return false;
        
        // Check if what we hit was the player
        var playerComponent = hit.collider.GetComponent<Player>();
        return playerComponent != null;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
        
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}