using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
    public bool isConnected { get; private set; }
    public AttachmentPoint ConnectedTo { get; set; }
    private SnapSystem snapSystem;

    public void Initialize(SnapSystem system)
    {
        snapSystem = system;
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

    public void SetConnected(bool status)
    {
        isConnected = status;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isConnected ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}