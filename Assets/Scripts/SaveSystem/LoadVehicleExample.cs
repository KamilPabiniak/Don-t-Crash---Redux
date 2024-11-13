using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadVehicleExample : MonoBehaviour
{
    public Transform spawnPoint; // Привязать точку спавна
    public VehicleLoader loader;

    //void Start()
    //{
    //    var path = Path.Combine(Application.dataPath, "Resources", "HellDiver.json");
    //    Debug.Log(path);
    //    loader.LoadFromFile(path, spawnPoint);
    //}
}