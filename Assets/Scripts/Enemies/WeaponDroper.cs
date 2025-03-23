using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class WeaponDroper : MonoBehaviour
{
    #region Inspector Properties
    [Description("Weapon Picker prefab used to spawn a new picker")]
    [SerializeField] private WeaponPicker picker;
    #endregion 
    
    #region Components
    private Health _health;
    #endregion

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += SpawnPicker;
    }

    private void SpawnPicker(Health health, Health.Change change)
    {
        if (change.JustDied(health))
            Debug.Log("Spawning picker");
    }
}
