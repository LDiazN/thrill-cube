using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Equipable))]
public class Throwable : MonoBehaviour
{
    #region Inspector Variables

    [Description("How much damage does this object do when hitting an enemy")] [SerializeField]
    private int damage;
    
    [Description("How much knockback does this object do when hitting an enemy")] [SerializeField]
    private float knockback;

    [Description("Sphere collider set as trigger used to detect the player")] [SerializeField]
    private TriggerEnterCallback pickRange;
    
    #endregion 
    
    #region Components
    Rigidbody _rigidbody;
    Equipable _equipable;
    public Equipable Equipable => _equipable;
    #endregion
    
    #region Internal State

    private bool _canHurt = false;
    [CanBeNull] public GameObject owner;
    
    #endregion
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _equipable = GetComponent<Equipable>();
    }

    private void OnEnable()
    {
        if (!pickRange)
            return;
        pickRange.OnTriggerEnterEvent += OnPickRangeTriggerEnter;
        pickRange.OnTriggerExitEvent += OnPickRangeTriggerExit;
    }
    
    private void OnDisable()
    {
        if (!pickRange)
            return;
        pickRange.OnTriggerEnterEvent -= OnPickRangeTriggerEnter;
        pickRange.OnTriggerExitEvent -= OnPickRangeTriggerExit;
    }

    public void SetCanHurt(bool canHurt) => _canHurt = canHurt;

    public void SetUpThrow()
    {
        SetCanHurt(true);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // If you can't hurt, do nothing
        if (!_canHurt)
            return;
        
        // If you can hurt, set this to not hurt anymore
        _canHurt = false;
        
        var health = collision.gameObject.GetComponent<Health>();
        if (!health)
            return;
        
        health.TakeDamage(damage, _rigidbody.linearVelocity.normalized, knockback, 3 * knockback, owner);
        if (health.isDead)
            PauseFor(0.25f);
    }

    private void OnPickRangeTriggerEnter(Collider other)
    {
        Debug.Log("Pick range trigger");
        var equipment = other.gameObject.GetComponent<Equipment>();
        if (!equipment)
            return;

        equipment.canPickFromFloor = _equipable;
    }

    private void OnPickRangeTriggerExit(Collider other)
    {
        var equipment = other.gameObject.GetComponent<Equipment>();
        if (!equipment)
            return;
        
        if (equipment.canPickFromFloor == _equipable)
            equipment.canPickFromFloor = null;
    }

    private void PauseFor(float seconds)
    {
        StartCoroutine(PauseTimeCoroutineFor(seconds));
    }

    private IEnumerator PauseTimeCoroutineFor(float seconds)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1;
    }
}
