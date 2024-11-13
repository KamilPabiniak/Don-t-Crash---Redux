using UnityEngine;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    public List<BlockType> blockTypes; 

    private Dictionary<string, BlockType> blockTypeDictionary;

    private void Awake()
    {
        blockTypeDictionary = new Dictionary<string, BlockType>();
        foreach (var blockType in blockTypes)
        {
            blockTypeDictionary.Add(blockType.blockName, blockType);
            Debug.Log(blockType.blockName);
        }
    }

    public BlockType GetBlockType(string blockName)
    {
        if (blockTypeDictionary.TryGetValue(blockName, out var blockType))
        {
            return blockType;
        }
        else
        {
            Debug.LogWarning("Block type not found: " + blockName);
            return null;
        }
    }
}
