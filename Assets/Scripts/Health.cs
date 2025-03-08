using System;
using System.ComponentModel;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Inspector Properties

    [Description("Max amount of health for this entity")] [SerializeField]
    private int _maxHealth = 10;

    #endregion

    #region Callbacks

    /// <summary>
    /// Called when the health changed. This component and the change in health are passed
    /// Arguments:
    ///     - Health component
    ///     - Health offset (negative means damage, positive means healing)
    ///     - Hit direction, in case of hit
    /// </summary>
    public event Action<Health, Change> OnHealthChanged;

    #endregion

    #region Internal State

    // We only serialize it to see it in inspector
    [SerializeField]
    int _currentHealth;

    /// <summary>
    /// Current amount of health
    /// </summary>
    public int CurrentHealth => _currentHealth;
    
    public bool isDead => _currentHealth <= 0;

    #endregion

    public struct Change
    {
        public float offset;
        public Vector3 direction;
        public float knockback;
        public float knockbackOnDead;

        public bool IsDamage => offset < 0;
        public bool IsHeal => knockback > 0;
    }

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage, Vector3 hitDirection = new Vector3(), float knockback = 0f, float knockbackOnDead = 0f)
    {
        var newHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        var offset = newHealth - CurrentHealth;
        _currentHealth = newHealth;
        
        OnHealthChanged?.Invoke(this, new Change{direction = hitDirection, offset = offset, knockback = knockback, knockbackOnDead = knockbackOnDead});
    }

    public void Heal(int healAmount)
    {
        // Heal is just negative damage
        TakeDamage(-healAmount);
    }
}
