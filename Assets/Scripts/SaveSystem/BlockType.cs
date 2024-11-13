using UnityEngine;

[CreateAssetMenu(fileName = "BlockType", menuName = "ScriptableObjects/BlockType", order = 1)]
public class BlockType : ScriptableObject
{
    public string blockName;
    public GameObject prefab;
    public int power;
    public int mass;
}