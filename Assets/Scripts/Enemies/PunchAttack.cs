using System;
using System.ComponentModel;
using UnityEngine;

public class PunchAttack : MonoBehaviour
{
    #region Inspector Properties
    
    [Description("How much damage to deal to the player")]
    [SerializeField]
    private int damage;

    [Description("How much force to apply to the player on a punch")] [SerializeField]
    private float knockbackForce;
    
    [SerializeField]
    [Description("Range detector used to check if can punch player")]
    RangeDetector rangeDetector;
    
    [SerializeField]
    [Description("How much time to wait before punches")]
    private float timeBetweenPunches = 0.5f;
    #endregion 
    
    #region Internal State
    public bool WantsToPunch = true;
    private float _timeSinceLastPunch;
    #endregion
    
    public void PunchPlayer(Player player)
    {
        var health = player.Health;
        var punchDirection = (player.transform.position - transform.position).normalized;
        health.TakeDamage(damage, punchDirection, knockbackForce, 2 * knockbackForce);
    }

    private void Update()
    {
        if (!WantsToPunch)
            return;
        
        _timeSinceLastPunch += Time.deltaTime;

        if (rangeDetector.IsPlayerInRange() && _timeSinceLastPunch >= timeBetweenPunches)
        {
            _timeSinceLastPunch = 0;            
            PunchPlayer(rangeDetector.Player);
        }
    }

}
