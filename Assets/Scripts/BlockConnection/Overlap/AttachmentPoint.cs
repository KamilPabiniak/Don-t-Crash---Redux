using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
    public bool isConnected = false; 
    public void SetConnected(bool status)
    {
        isConnected = status;
    }
    
    public Transform GetRootParent()
    {
        Transform current = transform;
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current;
    }

}