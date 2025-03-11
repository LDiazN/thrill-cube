using UnityEngine;

public class KnowledgeArea : MonoBehaviour
{
    #region Current Knowledge
    // A pointer to the player if it was detected 
    public Player player;
    #endregion
    
    #region Internal State

    public int knownAgents = 0;
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        // Set the knowledge area for this enemy 
        // to be this one
        var perceptions = other.GetComponent<Perceptions>();
        if (perceptions == null)
            return;
        perceptions.knowledgeArea = this;
        knownAgents++;
    }

    private void OnTriggerExit(Collider other)
    {
        // If this enemy has this knowledge area in its perception
        // and it's coming out of the area, clear it
        var perceptions = other.GetComponent<Perceptions>();
        if (perceptions == null)
            return;

        if (perceptions.knowledgeArea == this)
        {
            knownAgents--;
            perceptions.knowledgeArea = null;
            
            // If there's no one left, there's no one to tell anything to new 
            // enemies
            if (knownAgents == 0)
                ClearKnowledge();
        }
    }

    private void ClearKnowledge()
    {
        player = null;
    }
}
