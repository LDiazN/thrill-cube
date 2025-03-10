using UnityEngine;

public class KnowledgeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " entered");
    }
}
