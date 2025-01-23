using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
    public bool isConnected = false;
    private void OnTriggerStay(Collider other)
    {
        AttachmentPoint point = other.GetComponent<AttachmentPoint>();
        if (point)
        {
            SetConnected(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AttachmentPoint point = other.GetComponent<AttachmentPoint>();
        if (!point)
        {
            SetConnected(false);
        }
    }
    
    private void SetConnected(bool status)
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