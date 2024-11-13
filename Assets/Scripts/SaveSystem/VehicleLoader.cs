using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class VehicleLoader : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public string blockName; // Имя типа блока
        public Vector3 relativePosition;
        public Quaternion rotation;
        public List<SaveData> children = new List<SaveData>();
    }

    public BlockManager blockManager; // Ссылка на менеджер блоков
    private List<GameObject> loadedBlocks = new List<GameObject>(); // Список загруженных блоков

    public void LoadFromFile(string path, Transform parent)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("JSON Content: " + json);
        
                //SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
                //Debug.Log("Deserialization successful. Root block name: " + data.blockName);
                //InstantiateFromData(data, parent);
                CreateJoints(); // Создание соединений после загрузки всех блоков
            
    
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }

    private void InstantiateFromData(SaveData data, Transform parent)
    {
        if (data.blockName == null)
        {
            Debug.LogError("Block name is null!");
            return;
        }

        BlockType blockType = blockManager.GetBlockType(data.blockName);
        if (blockType == null)
        {
            Debug.LogError("Block type not found: " + data.blockName);
            return;
        }

        GameObject obj = Instantiate(blockType.prefab, parent);
        obj.transform.localPosition = data.relativePosition;
        obj.transform.localRotation = data.rotation;
        loadedBlocks.Add(obj); // Добавление блока в список загруженных блоков
        Debug.Log($"Created object of type {data.blockName} at relative position {data.relativePosition} with rotation {data.rotation}");

        foreach (var childData in data.children)
        {
            InstantiateFromData(childData, obj.transform);
        }
    }

    private void CreateJoints()
    {
        float jointDistanceThreshold = 3.0f; // Порог расстояния для создания соединения
        for (int i = 0; i < loadedBlocks.Count; i++)
        {
            for (int j = i + 1; j < loadedBlocks.Count; j++)
            {
                if (Vector3.Distance(loadedBlocks[i].transform.position, loadedBlocks[j].transform.position) <= jointDistanceThreshold)
                {
                    CreateJoint(loadedBlocks[i], loadedBlocks[j]);
                }
            }
        }
    }

    private void CreateJoint(GameObject obj1, GameObject obj2)
    {
        SpringJoint joint = obj1.AddComponent<SpringJoint>();
        joint.connectedBody = obj2.GetComponent<Rigidbody>();

        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.zero;
        joint.damper = 0.03f;
        joint.spring = 250f;
        joint.minDistance = 0.02f;
        joint.maxDistance = 0.55f;
        joint.tolerance = 0f;
        joint.enableCollision = true;

        if (joint.connectedBody == null)
        {
            Debug.LogError("Connected object does not have a Rigidbody: " + obj2.name);
        }
    }
}
