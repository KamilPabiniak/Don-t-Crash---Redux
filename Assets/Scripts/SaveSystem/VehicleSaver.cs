using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VehicleSaver : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public string blockName;
        public Vector3 relativePosition;
        public Quaternion rotation;
        public List<SaveData> children = new List<SaveData>();
    }

    public SaveData Save(Transform obj, Transform parent)
    {
        SaveData data = new SaveData();
        data.blockName = obj.GetComponent<BlockTypeInGame>().blockType.blockName;

        data.relativePosition = parent == null ? obj.position : parent.InverseTransformPoint(obj.position);
        data.rotation = obj.localRotation;

        foreach (Transform child in obj)
        {
            if (child.TryGetComponent<SnappingSystem>(out SnappingSystem component))
                data.children.Add(Save(child, obj));
        }

        return data;
    }

    public void SaveToFile(Transform vehicleRoot, string fileName)
    {
        SaveData data = Save(vehicleRoot, null);
        string json = JsonUtility.ToJson(data, true);
        var path = Path.Combine(Application.dataPath, "Resources", fileName+".json");
        File.WriteAllText(path, json);
    }
}
