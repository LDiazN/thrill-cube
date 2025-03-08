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
    
    #endregion 
    
    #region Internal State
    public bool WantsToPunch = true;
    #endregion

    public void PunchPlayer(Player player)
    {
        var health = player.Health;
        var punchDirection = (player.transform.position - transform.position).normalized;
        health.TakeDamage(damage, punchDirection, knockbackForce, 2 * knockbackForce);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!WantsToPunch)
            return;
        
        var player = other.gameObject.GetComponent<Player>();
        if (player == null)
            return;
        
        PunchPlayer(player);
    }
}
