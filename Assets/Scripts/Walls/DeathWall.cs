using System.ComponentModel;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    #region Inspector Properties
    [Description("The time it takes to the dead wall to reach the win wall")]
    [Min(0.1f)]
    [SerializeField] private float totalTime = 1.5f;
    
    [Description("Knockback force to apply to the player")]
    [SerializeField] private float knockbackForce = 50f;

    [Description("Blue wall used as target for the player")]
    [SerializeField] private WinWall target;
    #endregion
    
    #region Internal State
    private float _remTime;
    #endregion

    private void Reset()
    {
        target = FindFirstObjectByType<WinWall>();
    }

    private void Start()
    {
        _remTime = totalTime;
    }

    private void Update()
    {
        if (_remTime <= 0)
            return;
            
        var direction = target.transform.position - transform.position;
        var distance = direction.magnitude;
        direction.Normalize();
        var speed = distance / _remTime;
        
        transform.Translate( speed * Time.deltaTime * direction);
        _remTime = Mathf.Clamp(_remTime - Time.deltaTime, 0f, totalTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;

        var direction = GetDirection() + Vector3.up;
        direction.Normalize();
        player.Health.Kill(direction, knockbackForce, knockbackForce);
        player.Rigidbody.constraints = RigidbodyConstraints.None;
    }
    
    private Vector3 GetDirection() => (target.transform.position - transform.position).normalized;
}
