using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameIsStarted = false;
    public Transform vehicleRoot; // Przypisz główny obiekt pojazdu w edytorze
    public Transform spawnPoint; // Привязать точку спавна


    //string path = string.Empty;

    void Start()
    {
        //path = Path.Combine(Application.dataPath, "Resources", ".json");
    }


    public void GameStart()
    {
        
    }
}
