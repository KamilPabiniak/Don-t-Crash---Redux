using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameIsStarted = false;
    public Transform vehicleRoot; // Przypisz główny obiekt pojazdu w edytorze
    public Transform spawnPoint; // Привязать точку спавна
    public VehicleLoader loader;
    public VehicleSaver saver;

    //string path = string.Empty;

    void Start()
    {
        //path = Path.Combine(Application.dataPath, "Resources", ".json");
    }

    public void SaveVehicle(string fileName)
    {
        saver.SaveToFile(vehicleRoot, fileName);
    }


    public void LoadVehicle(string fileName)
    {
        var path = Path.Combine(Application.dataPath, "Resources", fileName+".json");
        loader.LoadFromFile(path, spawnPoint);
    }
    
    public void GameStart()
    {
        
    }
}
