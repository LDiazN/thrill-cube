using System;
using System.ComponentModel;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    #region Inspector Properties

    [Description("How fast it spins, in degrees")] [SerializeField]
    private float spinSpeed = 45f;

    [Description("How fast it floats in the Y axis")] [SerializeField] [Min(0)]
    private float floatSpeed = 1;
    
    [Description("How far it floats upwards or downwards")] [SerializeField] [Min(0)]
    private float amplitude = 1;

    #endregion
    
    #region Internal State
    private Vector3 _originalPosition;
    #endregion

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        var offset = amplitude * Mathf.Sin(floatSpeed * Time.timeSinceLevelLoad);
        var newPosition = _originalPosition + Vector3.up * offset;
        transform.localPosition = newPosition;
    }
}
