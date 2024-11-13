using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
    public bool isConnected = false; 
    
   
    public void SetConnected(bool status)
    {
        isConnected = status;
    }
}