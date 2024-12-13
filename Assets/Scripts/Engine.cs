using UnityEngine;

public class Engine : MonoBehaviour
{
    [Header("Engine Properties")]
    public float power = 100f; 

    public void StartEngine()
    {
        Debug.Log("Engine started!");
    }
}
