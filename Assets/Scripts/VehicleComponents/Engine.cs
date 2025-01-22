using UnityEngine;

public class Engine : MonoBehaviour
{
    [Header("Engine Properties")]
    public float power = 10f; 

    public void StartEngine()
    {
        Debug.Log("Engine started!");
    }
}
