using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapSystem : MonoBehaviour
{
    [SerializeField] public bool isRoot = false;  
    [SerializeField] private float snapRange = 0.3f;  
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
        AttachmentPoint closestPoint = FindClosestAvailableSnapPoint();
        Debug.Log(closestPoint);
        if (closestPoint != null)
        {
            SnapToPoint(closestPoint);
        }
        else
        {
            Detach(true); 
        }
    }

    private AttachmentPoint FindClosestAvailableSnapPoint()
    {
        AttachmentPoint closestPoint = null;
        float closestDistance = Mathf.Infinity;
        
        Transform rootTransform = transform.root;

        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, snapRange, snapLayerMask);

        foreach (Collider collider in nearbyColliders)
        {
            AttachmentPoint point = collider.GetComponent<AttachmentPoint>();
            
            if (point != null && !point.isConnected && point.GetRootParent() != rootTransform)
            {
                float distance = Vector3.Distance(transform.position, point.transform.position);
                Debug.Log($"Rozważany punkt: {point.name}, odległość: {distance}");
                if (distance < closestDistance)
                {
                    closestPoint = point;
                    closestDistance = distance;
                }
            }
            else if (point != null)
            {
                Debug.Log($"Pominięty punkt: {point.name}");
            }
        }

        if (closestPoint == null)
        {
            Debug.Log("Nie znaleziono żadnego dostępnego punktu snapowania.");
        }

        return closestPoint;
    }

    private void SnapToPoint(AttachmentPoint point)
    {
        transform.position = point.transform.position;
        transform.SetParent(point.transform);

        if (rb != null)
        {
            rb.isKinematic = true; 
        }
        
        float targetYAngle = Mathf.Round(transform.eulerAngles.y / 90f) * 90f;
        transform.rotation = Quaternion.Euler(0f, targetYAngle, 0f);

   
        point.SetConnected(true);
    }

    
    public void Detach(bool breakConnection)
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            AttachmentPoint point = currentParent.GetComponent<AttachmentPoint>();
            if (point != null && breakConnection)
            {
                point.SetConnected(false);
            }
            transform.SetParent(null);
            currentParent = currentParent.parent; 
        }

        if (rb != null)
        {
            rb.isKinematic = false; 
        }
    }

}
