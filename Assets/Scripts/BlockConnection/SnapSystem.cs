using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapSystem : MonoBehaviour
{
    [SerializeField] public bool isRoot = false;
    private const float SnapRange = 0.1f;
    [SerializeField] private LayerMask snapLayerMask;  
    [Header("Attachment Points")]
    [SerializeField] private AttachmentPoint[] snapPoints;

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable xrGrab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        xrGrab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (isRoot)
        {
            rb.isKinematic = true;  
            if (xrGrab != null)
            {
                xrGrab.enabled = false;  
            }
        }
        else if (xrGrab != null)
        {
            xrGrab.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDestroy()
    {
        if (xrGrab != null)
        {
            xrGrab.selectExited.RemoveListener(OnRelease);
        }
    }
    
    private void OnRelease(SelectExitEventArgs args) =>   TrySnap();
    
    private void TrySnap()
    {
        foreach (var myPoint in snapPoints)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(myPoint.transform.position, SnapRange, snapLayerMask);
            foreach (var collider in nearbyColliders)
            {
                var targetPoint = collider.GetComponent<AttachmentPoint>();
                
                if (targetPoint != null && targetPoint.GetRootParent() != myPoint.GetRootParent())
                {
                    SnapToPoint(myPoint, targetPoint);
                    return;
                }
            }
        }
        
        Detach();
    }
    
    private void SnapToPoint(AttachmentPoint myPoint, AttachmentPoint targetPoint)
    {
        transform.position = targetPoint.transform.position;
        transform.SetParent(targetPoint.transform);
        
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        Vector3 roundedRotation = new Vector3(
            Mathf.Round(transform.eulerAngles.x / 90f) * 90f,
            Mathf.Round(transform.eulerAngles.y / 90f) * 90f,
            Mathf.Round(transform.eulerAngles.z / 90f) * 90f
        );
        transform.rotation = Quaternion.Euler(roundedRotation);

        Debug.Log($"{myPoint.name} snapped to {targetPoint.name}");
    }

    
    public void Detach()
    {
        transform.SetParent(null);
        
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
