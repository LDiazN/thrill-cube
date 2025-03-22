using UnityEngine;

[RequireComponent(typeof(Knockback))]
public class DebugKnockback : MonoBehaviour
{
    #region Inspector properties

    [Min(0)]
    [SerializeField] private float knockbackForce;
    #endregion
    #region Components
    Knockback _knockback;
    #endregion
    
    private void Awake()
    {
        _knockback = GetComponent<Knockback>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            _knockback.Knock(knockbackForce, Vector3.forward);
    }
}
