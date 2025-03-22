using UnityEngine;

[CreateAssetMenu(fileName = "TestScriptable", menuName = "Scriptable Objects/TestScriptable")]
public class TestScriptable : ScriptableObject
{
    public int id;
    public int rarity;
    public string name;
}
